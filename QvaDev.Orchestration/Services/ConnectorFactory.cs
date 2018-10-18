﻿using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Services;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
	public class ConnectorFactory : IConnectorFactory
	{
		private readonly CTraderIntegration.ICtConnectorFactory _ctConnectorFactory;
		private readonly ILog _log;
		private readonly IEmailService _emailService;

		public ConnectorFactory(
			CTraderIntegration.ICtConnectorFactory ctConnectorFactory,
			IEmailService emailService,
			ILog log)
		{
			_emailService = emailService;
			_log = log;
			_ctConnectorFactory = ctConnectorFactory;
		}
		public async Task Create(Account account)
		{
			if (account.MetaTraderAccountId.HasValue) ConnectMtAccount(account);
			if (account.CTraderAccountId.HasValue) ConenctCtAccount(account);
			if (account.FixTraderAccountId.HasValue) ConnectFtAccount(account);
			if (account.FixApiAccountId.HasValue) await ConnectFixAccount(account);
			if (account.IlyaFastFeedAccountId.HasValue) ConnectIlyaFastFeedAccount(account);
			if (account.CqgClientApiAccountId.HasValue) ConnectCqgClientApiAccount(account);
		}

		private void ConnectMtAccount(Account account)
		{
			void DestinationSetter(string d) => account.Destination = d;

			if (!(account.Connector is Mt4Integration.Connector) ||
			    account.Connector.Id != account.MetaTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new Mt4Integration.Connector(_log);
			((Mt4Integration.Connector) account.Connector)
				.Connect(new Mt4Integration.AccountInfo()
				{
					DbId = account.MetaTraderAccount.Id,
					Description = account.MetaTraderAccount.Description,
					User = (uint) account.MetaTraderAccount.User,
					Password = account.MetaTraderAccount.Password,
					Srv = account.MetaTraderAccount.MetaTraderPlatform.SrvFilePath
				}, DestinationSetter);
		}

		private void ConenctCtAccount(Account account)
		{
			if (!(account.Connector is CTraderIntegration.Connector) ||
			    account.Connector.Id != account.CTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
			{
				account.Connector = (CTraderIntegration.Connector)_ctConnectorFactory.Create(
					new CTraderIntegration.PlatformInfo
					{
						Description = account.CTraderAccount.CTraderPlatform.Description,
						AccountsApi = account.CTraderAccount.CTraderPlatform.AccountsApi,
						ClientId = account.CTraderAccount.CTraderPlatform.ClientId,
						TradingHost = account.CTraderAccount.CTraderPlatform.TradingHost,
						Secret = account.CTraderAccount.CTraderPlatform.Secret,
						Playground = account.CTraderAccount.CTraderPlatform.Playground
					},
					new CTraderIntegration.AccountInfo
					{
						DbId = account.CTraderAccount.Id,
						Description = account.CTraderAccount.Description,
						AccountNumber = account.CTraderAccount.AccountNumber,
						AccessToken = account.CTraderAccount.AccessToken
					});
			}
			((CTraderIntegration.Connector)account.Connector).Connect();
		}

		private void ConnectFtAccount(Account account)
		{
			if (!(account.Connector is FixTraderIntegration.Connector) ||
			    account.Connector.Id != account.FixTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new FixTraderIntegration.Connector(_log);

			((FixTraderIntegration.Connector) account.Connector)
				.Connect(new FixTraderIntegration.AccountInfo
				{
					DbId = account.FixTraderAccount.Id,
					Description = account.FixTraderAccount.Description,
					IpAddress = account.FixTraderAccount.IpAddress,
					CommandSocketPort = account.FixTraderAccount.CommandSocketPort,
					EventsSocketPort = account.FixTraderAccount.EventsSocketPort
				});
		}

		private async Task ConnectFixAccount(Account account)
		{
			if (!(account.Connector is FixApiIntegration.Connector) ||
			    account.Connector.Id != account.FixTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new FixApiIntegration.Connector(new FixApiIntegration.AccountInfo
				{
					DbId = account.FixApiAccount.Id,
					Description = account.FixApiAccount.Description,
					ConfigPath = account.FixApiAccount.ConfigPath,
				}, _emailService, _log);

			await ((FixApiIntegration.Connector) account.Connector).Connect();
		}

		private async void ConnectIlyaFastFeedAccount(Account account)
		{
			if (!(account.Connector is IlyaFastFeedIntegration.Connector) ||
			    account.Connector.Id != account.IlyaFastFeedAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new IlyaFastFeedIntegration.Connector(_log);

			await ((IlyaFastFeedIntegration.Connector)account.Connector).Connect(new IlyaFastFeedIntegration.AccountInfo()
			{
				DbId = account.IlyaFastFeedAccount.Id,
				Description = account.IlyaFastFeedAccount.Description,
				IpAddress = account.IlyaFastFeedAccount.IpAddress,
				Port = account.IlyaFastFeedAccount.Port,
				UserName = account.IlyaFastFeedAccount.UserName
			});
		}

		private async void ConnectCqgClientApiAccount(Account account)
		{
			if (!(account.Connector is CqgClientApiIntegration.Connector) ||
			    account.Connector.Id != account.IlyaFastFeedAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new CqgClientApiIntegration.Connector(new CqgClientApiIntegration.AccountInfo()
				{
					DbId = account.CqgClientApiAccount.Id,
					Description = account.CqgClientApiAccount.Description,
					UserName = account.CqgClientApiAccount.UserName,
					Password = account.CqgClientApiAccount.Password
				}, _log);

			await ((CqgClientApiIntegration.Connector)account.Connector).Connect();
		}
	}
}
