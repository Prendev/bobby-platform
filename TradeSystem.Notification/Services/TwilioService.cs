using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using TradeSystem.Common.Integration;

namespace TradeSystem.Notification.Services
{
	public interface ITwilioService
	{
		void Start();
		void Stop();
		void AddAccount(Account account);
	}

	public class TwilioService : ITwilioService
	{
		private ConcurrentDictionary<Account, SemaphoreSlim> accountSemaphoreMarginError;

		private ConcurrentDictionary<Account, int> accountErrorStateInMins;
		private ConcurrentDictionary<Account, SemaphoreSlim> accountSemaphoreDisconnectError;

		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public void Start()
		{
			cancellationTokenSource = new CancellationTokenSource();
			accountErrorStateInMins = new ConcurrentDictionary<Account, int>();
			accountSemaphoreMarginError = new ConcurrentDictionary<Account, SemaphoreSlim>();
			accountSemaphoreDisconnectError = new ConcurrentDictionary<Account, SemaphoreSlim>();
		}

		public void Stop()
		{
			cancellationTokenSource.Cancel();
			accountSemaphoreMarginError = null;
			accountSemaphoreDisconnectError = null;
			accountErrorStateInMins = null;
		}

		public void AddAccount(Account account)
		{
			account.MarginChanged += Account_MarginChanged;
			account.ConnectionChanged += Account_ConnectionChanged; ;
			accountSemaphoreMarginError.TryAdd(account, new SemaphoreSlim(1, 1));
			accountSemaphoreDisconnectError.TryAdd(account, new SemaphoreSlim(1, 1));
			accountErrorStateInMins.TryAdd(account, 0);
		}

		private async void Account_MarginChanged(object sender, EventArgs e)
		{
			var account = sender as Account;
			if (!account.IsAlert) return;

			await accountSemaphoreMarginError[account].WaitAsync();
			try
			{
				if (account.IsAlert &&
					account.Connector.MarginLevel < account.MarginLevelAlert &&
					!(account.Connector.Margin == 0 && account.Connector.MarginLevel == 0))
				{
					await SendTwilioNotifications(account);
				}
			}
			catch (OperationCanceledException) { }
			finally
			{
				if (accountSemaphoreMarginError != null && accountSemaphoreMarginError.ContainsKey(account))
					accountSemaphoreMarginError[account].Release();
			}
		}

		private async void Account_ConnectionChanged(object sender, Common.Integration.ConnectionStates e)
		{
			var account = sender as Account;

			if (e == ConnectionStates.Connected)
			{
				accountErrorStateInMins[account] = 0;
			}
			else if (e == ConnectionStates.Error && account.HasAlreadyConnected && account.DisconnectAlert != null)
			{
				await DisconnectError(account);
			}
		}

		private async Task SendTwilioNotifications(Account account)
		{
			var coolDownInMin = 1;

			using (var context = new DuplicatContext())
			{
				context.TwilioSettings.Load();
				context.TwilioPhoneSettings.Load();

				var twilioSettings = context.TwilioSettings.ToList();
				var phoneSettings = context.TwilioPhoneSettings.Where(ps => ps.Active).ToList();

				var coolDownTimerInMin = twilioSettings.FirstOrDefault(ts => ts.Key.Equals(ConfigurationManager.AppSettings["TwilioService.CoolDownTimerInMin"]));

				if (!int.TryParse(coolDownTimerInMin.Value, out coolDownInMin))
				{
					TwilioLogger.Error($"Invalid value for twilio CoolDownTimerInMin property. Please provide a valid numerical value for the cooldown timer.");
					return;
				}

				if (!phoneSettings.Any())
				{
					TwilioLogger.Info($"The {account} account has triggered an alert.");
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


						TwilioClient.Init(accountSid.Value, authToken.Value);

						foreach (var phoneSetting in phoneSettings)
						{
							try
							{
								var call = CallResource.Create(
								twiml: new Twiml($"<Response><Say>{message?.Value ?? $"{account} account alert!"}</Say></Response>"),
								to: new PhoneNumber(phoneSetting.PhoneNumber),
								from: new PhoneNumber(twilioPhoneNumber.Value));

								TwilioLogger.Info($"The {account} account has triggered an alert. A call notification has been successfully sent to {phoneSetting.Name}.");
							}
							catch (Exception ex)
							{
								TwilioLogger.Error($"Unable to send notification for the {account} account alert due to an error. Please check your notification settings and ensure that the account details are correct.", ex);
							}
						}

					}
					catch (Exception twilioError)
					{
						TwilioLogger.Error($"There was an error with the Twilio credentials. Please review your API secrets.", twilioError);
					}
				}
			}

			await Task.Delay(coolDownInMin * /*60 **/ 1000);
		}

		private async Task DisconnectError(Account account)
		{
			await accountSemaphoreDisconnectError[account].WaitAsync();
			try
			{
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
								await SendTwilioNotifications(account);
							}
							break;
						case DisconnectAlert.WeekEnd_2hours:
							if (isWeekEnd && accountErrorStateInMins[account] > 120)
							{
								await SendTwilioNotifications(account);
							}
							break;
						case DisconnectAlert.WeekDays_3min_WeekEnd_2hours:
							if ((!isWeekEnd && accountErrorStateInMins[account] > 3) || (isWeekEnd && accountErrorStateInMins[account] > 120))
							{
								await SendTwilioNotifications(account);
							}
							break;
						case DisconnectAlert.AllDay_3min:
							if (accountErrorStateInMins[account] > 3)
							{
								await SendTwilioNotifications(account);
							}
							break;
					}

					await Task.Delay(/*60 **/ 1000);
				}

				if (accountErrorStateInMins != null)
				{
					accountErrorStateInMins[account] = 0;
				}
			}
			finally
			{
				if (accountSemaphoreDisconnectError != null)
					accountSemaphoreDisconnectError[account].Release();
			}
		}
	}
}
