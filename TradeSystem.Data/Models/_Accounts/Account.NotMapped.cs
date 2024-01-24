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

namespace TradeSystem.Data.Models
{
	public partial class Account
	{
		private bool notificationSent;
		private readonly Object _lock = new Object();
		private Timer cooldownTimer;

		private volatile bool _isBusy;
		private bool isUserEditing;

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

			cooldownTimer = new Timer();
			cooldownTimer.Elapsed += (sender, args) =>
			{
				// Timer has elapsed, set notify property to false
				notificationSent = false;

				// Stop the timer
				((Timer)sender).Stop();
			}; ;
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

		private void Connector_ConnectionChanged(object sender, ConnectionStates e)
		{
			ConnectionState = e;
			ConnectionChanged?.Invoke(this, ConnectionState);
		}

		private void Connector_NewPosition(object sender, NewPosition e) => NewPosition?.Invoke(this, e);
		private void Connector_LimitFill(object sender, LimitFill e) => LimitFill?.Invoke(this, e);

		private void Connector_MarginChanged(object sender, EventArgs e)
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

			lock (_lock)
			{
				if (!notificationSent && IsAlert && Connector.MarginLevel < MarginLevelAlert && !(Connector.Margin == 0 && Connector.MarginLevel == 0))
				{
					SendTwilioNotifications();
				}
			}
		}

		private void SendTwilioNotifications()
		{
			var coolDownInMin = 1;
			notificationSent = true;

			using (var context = new DuplicatContext())
			{
				context.TwilioSettings.Load();
				context.PhoneSettings.Load();

				var twilioSettings = context.TwilioSettings.ToList();
				var phoneSettings = context.PhoneSettings.Where(ps => ps.Active).ToList();

				if (!phoneSettings.Any())
				{
					Logger.Info($"The {MetaTraderAccount.Description} account has triggered an alert.");
					Logger.Warn($"A call notification hasn't been sent because there are no active phone numbers for notification.");
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
							Logger.Error($"Invalid value for twilio CoolDownTimerInMin property. Please provide a valid numerical value for the cooldown timer.");
							return;
						}

						TwilioClient.Init(accountSid.Value, authToken.Value);

						foreach (var phoneSetting in phoneSettings)
						{
							try
							{
								var call = CallResource.Create(
								twiml: new Twiml($"<Response><Say>{message?.Value ?? $"{MetaTraderAccount.Description} account alert!"}</Say></Response>"),
								to: new PhoneNumber(phoneSetting.PhoneNumber),
								from: new PhoneNumber(twilioPhoneNumber.Value));

								Logger.Info($"The {MetaTraderAccount.Description} account has triggered an alert. A call notification has been successfully sent to {phoneSetting.Name}.");
							}
							catch (Exception messageError)
							{
								Logger.Error($"Unable to send notification for the {MetaTraderAccount.Description} account alert due to an error. Please check your notification settings and ensure that the account details are correct.", messageError);
							}
						}

					}
					catch (Exception twilioError)
					{
						Logger.Error($"There was an error with the Twilio credentials. Please review your API secrets.", twilioError);
					}
				}
			}

			cooldownTimer.Interval = coolDownInMin * 60 * 1000;
			cooldownTimer.Start();
		}
	}
}
