using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Configuration;
using System.Timers;
using TradeSystem.Notification;
using Telegram.Bot;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TradeSystem.Data.Models
{
	public partial class Account
	{
		private readonly System.Threading.SemaphoreSlim semaphoreSlim;
		private readonly System.Threading.SemaphoreSlim disconnectedAccountSemaphoreSlim;

		private bool twilioNotificationSent;
		private bool telegramNotificationSent;
		private Timer twilioCooldownTimer;
		private Timer telegramCooldownTimer;

		private volatile bool _isBusy;
		private bool isUserEditing;

		private int errorStateInMin;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool IsUserEditing
		{
			get => isUserEditing;
			set => isUserEditing = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool HasAlreadyConnected { get; set; }

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;
		public event EventHandler<NewPosition> NewPosition;
		public event EventHandler<LimitFill> LimitFill;

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Balance { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Equity { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double PnL { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Margin { get => Get<double>(); set => Set(value); }

		[DisplayName("Free M")]
		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double FreeMargin { get => Get<double>(); set => Set(value); }

		[DisplayName("M %")]
		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double MarginLevel { get => Get<double>(); set => Set(value); }

		[NotMapped][InvisibleColumn] public IConnector Connector { get => Get<IConnector>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public string DestinationHost { get => Get<string>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public int DestinationPort { get => Get<int>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public int CooldownTimerInMin { get => Get<int>(); }

		public Account()
		{
			SetAction<IConnector>(nameof(Connector),
				x =>
				{
					if (x == null) return;
					x.NewTick -= Connector_NewTick;
					x.ConnectionChanged -= Connector_ConnectionChanged;
					x.NewPosition -= Connector_NewPosition;
					x.LimitFill -= Connector_LimitFill;
					x.MarginChanged -= Connector_MarginChanged;
				},
				x =>
				{
					if (x == null) return;
					x.NewTick += Connector_NewTick;
					x.ConnectionChanged += Connector_ConnectionChanged;
					x.NewPosition += Connector_NewPosition;
					x.LimitFill += Connector_LimitFill;
					x.MarginChanged += Connector_MarginChanged;
				});

			semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
			disconnectedAccountSemaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);

			twilioCooldownTimer = new Timer();
			twilioCooldownTimer.Elapsed += (sender, args) =>
			{
				// Timer has elapsed, set notify property to false
				twilioNotificationSent = false;

				// Stop the timer
				((Timer)sender).Stop();
			};

			telegramCooldownTimer = new Timer();
			telegramCooldownTimer.Elapsed += (sender, args) =>
			{
				// Timer has elapsed, set notify property to false
				telegramNotificationSent = false;

				// Stop the timer
				((Timer)sender).Stop();
			};

		}

		public Tick GetLastTick(string symbol) => Connector?.GetLastTick(symbol);

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CT | {CTraderAccount.Description}";
			if (FixApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IConn | {FixApiAccount.Description}";
			if (CqgClientApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CQG | {CqgClientApiAccount.Description}";
			if (IbAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IB | {IbAccount.Description}";
			if (BacktesterAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}BT | {BacktesterAccount.Description}";
			return "";
		}

		private void Connector_NewTick(object sender, NewTick e)
		{
			if (BacktesterAccount != null) BacktesterAccount.UtcNow = e.Tick.Time;
			NewTick?.Invoke(this, e);
		}
		private async void Connector_ConnectionChanged(object sender, ConnectionStates e)
		{
			ConnectionState = e;
			ConnectionChanged?.Invoke(this, ConnectionState);

			switch (e)
			{
				case ConnectionStates.Connected:
					HasAlreadyConnected = true;
					errorStateInMin = 0;
					break;
				case ConnectionStates.Error:
					if (disconnectedAccountSemaphoreSlim.CurrentCount == 0) return;
					await disconnectedAccountSemaphoreSlim.WaitAsync();
					try
					{
						if (HasAlreadyConnected && DisconnectAlert != null)
							await CheckDisconnectedError();
					}
					finally
					{
						disconnectedAccountSemaphoreSlim.Release();
					}
					break;
			}
		}

		private async Task CheckDisconnectedError()
		{
			while (ConnectionState == ConnectionStates.Error && HasAlreadyConnected)
			{
				var currentUtcMinusOneHour = DateTime.UtcNow.AddHours(-1);
				var isWeekEnd = currentUtcMinusOneHour.DayOfWeek == DayOfWeek.Saturday || currentUtcMinusOneHour.DayOfWeek == DayOfWeek.Sunday;

				errorStateInMin++;
				switch (DisconnectAlert)
				{
					case Common.Integration.DisconnectAlert.WeekDays_3min:
						if (!isWeekEnd && errorStateInMin > 3)
						{
							await SendNotifications();
						}
						break;
					case Common.Integration.DisconnectAlert.WeekEnd_2hours:
						if (isWeekEnd && errorStateInMin > 120)
						{
							await SendNotifications();
						}
						break;
					case Common.Integration.DisconnectAlert.WeekDays_3min_WeekEnd_2hours:
						if ((!isWeekEnd && errorStateInMin > 3) || (isWeekEnd && errorStateInMin > 120))
						{
							await SendNotifications();
						}
						break;
					case Common.Integration.DisconnectAlert.AllDay_3min:
						if (errorStateInMin > 3)
						{
							await SendNotifications();
						}
						break;
				}

				await Task.Delay(60 * 1000);
			}

			errorStateInMin = 0;
		}

		private void Connector_NewPosition(object sender, NewPosition e) => NewPosition?.Invoke(this, e);
		private void Connector_LimitFill(object sender, LimitFill e) => LimitFill?.Invoke(this, e);

		private async void Connector_MarginChanged(object sender, EventArgs e)
		{
			if (!isUserEditing)
			{
				Balance = Connector.Balance;
				Equity = Connector.Equity;
				PnL = Connector.PnL;
				Margin = Connector.Margin;
				FreeMargin = Connector.FreeMargin;
				MarginLevel = Connector.MarginLevel;
			}

			await semaphoreSlim.WaitAsync();
			try
			{
				if (Connector.MarginLevel < MarginLevelAlert && !(Connector.Margin == 0 && Connector.MarginLevel == 0))
					await SendNotifications();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		private async Task SendNotifications()
		{
			if (!IsAlert) return;
			if (!twilioNotificationSent)
			{
				SendTwilioNotifications();
			}

			if (!telegramNotificationSent)
			{
				await SendTelegramNotifications();
			}
		}

		private void SendTwilioNotifications()
		{
			var coolDownInMin = 1;
			twilioNotificationSent = true;

			using (var context = new DuplicatContext())
			{
				context.TwilioSettings.Load();
				context.TwilioPhoneSettings.Load();

				var twilioSettings = context.TwilioSettings.ToList();
				var phoneSettings = context.TwilioPhoneSettings.Where(ps => ps.Active).ToList();

				if (!phoneSettings.Any())
				{
					TwilioLogger.Info($"The {ToString()} account has triggered an alert.");
					TwilioLogger.Warn($"A call notification hasn't been sent because there are no active phone numbers for notification.");
				}
				else
				{
					try
					{
						var accountSid = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.AccountSid"]));
						var authToken = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.AuthToken"]));
						var twilioPhoneNumber = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.TwilioPhoneNumber"]));
						var message = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.Message"]));
						var coolDownTimerInMin = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.CoolDownTimerInMin"]));

						if (!int.TryParse(coolDownTimerInMin.Value, out coolDownInMin))
						{
							TwilioLogger.Error($"Invalid value for twilio CoolDownTimerInMin property. Please provide a valid numerical value for the cooldown timer.");
							return;
						}

						TwilioClient.Init(accountSid.Value, authToken.Value);

						foreach (var phoneSetting in phoneSettings)
						{
							try
							{
								var call = CallResource.Create(
								twiml: new Twiml($"<Response><Say>{message?.Value ?? $"{ToString()} account alert!"}</Say></Response>"),
								to: new PhoneNumber(phoneSetting.PhoneNumber),
								from: new PhoneNumber(twilioPhoneNumber.Value));

								TwilioLogger.Info($"The {ToString()} account has triggered an alert. A call notification has been successfully sent to {phoneSetting.Name}.");
							}
							catch (Exception ex)
							{
								TwilioLogger.Error($"Unable to send notification for the {ToString()} account alert due to an error. Please check your notification settings and ensure that the account details are correct.", ex);
							}
						}

					}
					catch (Exception twilioError)
					{
						TwilioLogger.Error($"There was an error with the Twilio credentials. Please review your API secrets.", twilioError);
					}
				}
			}

			twilioCooldownTimer.Interval = coolDownInMin * 60 * 1000;
			twilioCooldownTimer.Start();
		}

		private async Task SendTelegramNotifications()
		{
			var coolDownInMin = 1;
			telegramNotificationSent = true;

			using (var context = new DuplicatContext())
			{
				context.TelegramSettings.Load();
				context.TelegramChatSettings.Load();

				var telegramSettings = context.TelegramSettings.ToList();
				var telegramChatSettings = context.TelegramChatSettings.Where(ps => ps.Active).ToList();

				if (!telegramChatSettings.Any())
				{
					TelegramLogger.Info($"The {ToString()} account has triggered an alert.");
					TelegramLogger.Warn($"A telegram notification hasn't been sent because there are no active telegram chats for notification.");
				}
				else
				{
					try
					{
						var token = telegramSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TelegramService.Token"]));
						var message = telegramSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TelegramService.Message"]));
						var coolDownTimerInMin = telegramSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TelegramService.CoolDownTimerInMin"]));

						if (!int.TryParse(coolDownTimerInMin.Value, out coolDownInMin))
						{
							TelegramLogger.Error($"Invalid value for telegram CoolDownTimerInMin property. Please provide a valid numerical value for the cooldown timer.");
							return;
						}

						var botClient = new TelegramBotClient(token.Value);

						foreach (var telegramChatSetting in telegramChatSettings)
						{
							try
							{
								await botClient.SendTextMessageAsync(telegramChatSetting.ChatId, $"{ToString()} account alert!\n{message?.Value}");
								TelegramLogger.Info($"The {ToString()} account has triggered an alert. A telegram notification has been successfully sent to chat id: '{telegramChatSetting.ChatId}'.");
							}
							catch (Exception ex)
							{
								TelegramLogger.Error($"Unable to send notification for the {ToString()} account alert due to an error. Please check your notification settings and ensure that the account details are correct.\n{ex.Message}");
							}
						}

					}
					catch (Exception ex)
					{
						TelegramLogger.Error($"There was an error with the Telegram credentials. Please review your API secrets.", ex);
					}
				}
			}

			telegramCooldownTimer.Interval = coolDownInMin * 60 * 1000;
			telegramCooldownTimer.Start();
		}
	}
}
