using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Notification.Services
{
	public interface ITelegramService
	{
		void Start(List<Account> accounts, List<TelegramChatSetting> telegramChatSettings);
		void Stop();
	}

	public class TelegramService : ITelegramService
	{
		private bool isSetup;
		private List<Account> telegramAccounts;

		private List<TelegramChatSetting> telegramChatSettings;

		private ConcurrentDictionary<(Account, TelegramChatSetting), bool> telegramAccountChatSettings;

		private ConcurrentDictionary<Account, int> accountErrorStateInMins;

		private CancellationTokenSource cancellationTokenSource;

		public event EventHandler<(Account, TelegramChatSetting)> MarginErrorEvent;
		public event EventHandler<(Account, TelegramChatSetting)> DisconnectErrorEvent;
		public event EventHandler<(Account, TelegramChatSetting)> LowHighEquityErrorEvent;
		public event EventHandler<(Account, TelegramChatSetting)> HighestTicketDurationErrorEvent;

		public void Start(List<Account> accounts, List<TelegramChatSetting> telegramChatSettings)
		{
			this.telegramChatSettings = telegramChatSettings;

			if (!isSetup)
			{
				isSetup = true;
				telegramAccounts = new List<Account>();
				cancellationTokenSource = new CancellationTokenSource();
				accountErrorStateInMins = new ConcurrentDictionary<Account, int>();
				telegramAccountChatSettings = new ConcurrentDictionary<(Account, TelegramChatSetting), bool>();

				DisconnectErrorEvent += TelegramService_DisconnectErrorEvent;
				MarginErrorEvent += TelegramService_BasicErrorEvent;
				LowHighEquityErrorEvent += TelegramService_BasicErrorEvent;
				HighestTicketDurationErrorEvent += TelegramService_BasicErrorEvent;
			}

			var validAccounts = accounts.Where(acc => acc.IsValidAccount()).ToList();
			telegramAccounts.AddRange(validAccounts);

			validAccounts.ForEach(account =>
			{
				telegramChatSettings.ForEach(tcs =>
				{
					telegramAccountChatSettings.TryAdd((account, tcs), false);
				});

				accountErrorStateInMins.TryAdd(account, 0);

				account.MarginChanged += Account_MarginChanged;
				account.ConnectionChanged += Account_ConnectionChanged;
			});
		}

		public void Stop()
		{
			isSetup = false;

			telegramAccounts.ForEach(account =>
			{
				{
					account.MarginChanged -= Account_MarginChanged;
					account.ConnectionChanged -= Account_ConnectionChanged;
				}
			});

			DisconnectErrorEvent -= TelegramService_DisconnectErrorEvent;
			MarginErrorEvent -= TelegramService_BasicErrorEvent;
			LowHighEquityErrorEvent -= TelegramService_BasicErrorEvent;
			HighestTicketDurationErrorEvent -= TelegramService_BasicErrorEvent;

			cancellationTokenSource.Cancel();
			telegramAccountChatSettings = null;
			accountErrorStateInMins = null;
		}

		private void Account_MarginChanged(object sender, EventArgs e)
		{
			var account = sender as Account;

			lock (account)
			{
				foreach (var telegramKeyValue in telegramAccountChatSettings)
				{
					var (keyAccount, keyTelegramChatSetting) = telegramKeyValue.Key;
					if (keyAccount != account || keyTelegramChatSetting.Active == false || telegramKeyValue.Value) continue;

					if (keyTelegramChatSetting.NotificationType == NotificationType.Account_Margin_Error &&
						!(account.Connector.Margin == 0 && account.Connector.MarginLevel == 0) &&
						account.IsAlert &&
						account.Connector.MarginLevel < account.MarginLevelAlert)
					{
						MarginErrorEvent?.Invoke(this, telegramKeyValue.Key);
					}

					if (keyTelegramChatSetting.NotificationType == NotificationType.HighLowEquity && (
						(account.RiskManagement.LowEquity.HasValue && account.RiskManagement.LowEquity > 0 && account.Equity < account.RiskManagement.LowEquity.Value) ||
						(account.RiskManagement.HighEquity.HasValue && account.RiskManagement.HighEquity > 0 && account.Equity > account.RiskManagement.HighEquity.Value)))
					{
						LowHighEquityErrorEvent?.Invoke(this, telegramKeyValue.Key);
					}

					if (keyTelegramChatSetting.NotificationType == NotificationType.HighestTicketDuration &&
						account.RiskManagement.HighestTicketDuration.HasValue &&
						account.RiskManagement.HighestTicketDuration.Value >= account.RiskManagement.RiskManagementSetting.MaxTicketDuration)
					{
						HighestTicketDurationErrorEvent?.Invoke(this, telegramKeyValue.Key);
					}
				}
			}
		}

		private async void TelegramService_BasicErrorEvent(object sender, (Account, TelegramChatSetting) telegramKey)
		{
			telegramAccountChatSettings[telegramKey] = true;
			try
			{
				await SendNotificationToTelegramChat(telegramKey);
				await Task.Delay(1000 * 60 * telegramKey.Item2.CoolDownTimerInMin, cancellationTokenSource.Token);
			}
			catch (OperationCanceledException) { }

			if (telegramAccountChatSettings != null)
				telegramAccountChatSettings[telegramKey] = false;
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates e)
		{
			var account = sender as Account;

			if (e == ConnectionStates.Connected)
			{
				accountErrorStateInMins[account] = 0;
			}
			else if (e == ConnectionStates.Error && account.HasAlreadyConnected && account.DisconnectAlert != null)
			{
				lock (account)
				{
					foreach (var telegramKeyValue in telegramAccountChatSettings)
					{
						if (telegramKeyValue.Key.Item1 == account && telegramKeyValue.Key.Item2.Active &&
							telegramKeyValue.Key.Item2.NotificationType == NotificationType.Account_Disconnection && !telegramKeyValue.Value)
						{
							DisconnectErrorEvent?.Invoke(this, telegramKeyValue.Key);
						}
					}
				}
			}
		}

		private async Task SendNotificationToTelegramChat((Account, TelegramChatSetting) telegramKeyValue)
		{
			var (account, tcs) = telegramKeyValue;

			var botClient = new TelegramBotClient(tcs.TelegramBot.Token);
			try
			{
				await botClient.SendTextMessageAsync(tcs.ChatId, $"{account} account alert!\n{tcs.Message}");
				TelegramLogger.Info($"The {account} account has triggered a [{tcs.NotificationType}] alert. A telegram notification has been successfully sent to chat id: '{tcs.ChatId}'.");
			}
			catch (Exception ex)
			{
				TelegramLogger.Error($"Unable to send notification for the {account} account alert due to an error. Please check your notification settings and ensure that the account details are correct.\n{ex.Message}");
			}
		}

		private async void TelegramService_DisconnectErrorEvent(object sender, (Account, TelegramChatSetting) telegramKeyValue)
		{
			telegramAccountChatSettings[telegramKeyValue] = true;
			var (account, tcs) = telegramKeyValue;
			while (account.ConnectionState == ConnectionStates.Error && account.HasAlreadyConnected && !cancellationTokenSource.IsCancellationRequested)
			{
				var currentUtcMinusOneHour = DateTime.UtcNow.AddHours(-1);
				var isWeekEnd = currentUtcMinusOneHour.DayOfWeek == DayOfWeek.Saturday || currentUtcMinusOneHour.DayOfWeek == DayOfWeek.Sunday;

				accountErrorStateInMins[account]++;
				switch (account.DisconnectAlert)
				{
					case DisconnectAlert.WeekDays_3min:
						if (!isWeekEnd && accountErrorStateInMins[account] > 3)
						{
							await SendDisconnectErrorNotification(telegramKeyValue);
						}
						break;
					case DisconnectAlert.WeekEnd_2hours:
						if (isWeekEnd && accountErrorStateInMins[account] > 120)
						{
							await SendDisconnectErrorNotification(telegramKeyValue);
						}
						break;
					case DisconnectAlert.WeekDays_3min_WeekEnd_2hours:
						if ((!isWeekEnd && accountErrorStateInMins[account] > 3) || (isWeekEnd && accountErrorStateInMins[account] > 120))
						{
							await SendDisconnectErrorNotification(telegramKeyValue);
						}
						break;
					case DisconnectAlert.AllDay_3min:
						if (accountErrorStateInMins[account] > 3)
						{
							await SendDisconnectErrorNotification(telegramKeyValue);
						}
						break;
				}

				await Task.Delay(60 * 1000);
			}

			if (accountErrorStateInMins != null)
			{
				accountErrorStateInMins[account] = 0;
			}
			if (telegramAccountChatSettings != null)
				telegramAccountChatSettings[telegramKeyValue] = false;
		}

		private async Task SendDisconnectErrorNotification((Account, TelegramChatSetting) telegramKey)
		{
			var (account, tcs) = telegramKey;

			await SendNotificationToTelegramChat(telegramKey);
			cancellationTokenSource.Token.ThrowIfCancellationRequested();

			try
			{
				// -1min because errorStateInMin gives an extra min
				var coolDown = 1000 * 60 * (tcs.CoolDownTimerInMin - 1);
				if (coolDown > 0)
				{
					await Task.Delay(coolDown, cancellationTokenSource.Token);
				}
			}
			catch (OperationCanceledException) { }
		}
	}
}
