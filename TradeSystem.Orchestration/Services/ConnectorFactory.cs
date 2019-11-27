using System;
using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Common.Services;
using TradeSystem.Data;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public class ConnectorFactory : IConnectorFactory
	{
		private readonly CTraderIntegration.ICtConnectorFactory _ctConnectorFactory;
		private readonly IEmailService _emailService;

		public ConnectorFactory(
			CTraderIntegration.ICtConnectorFactory ctConnectorFactory,
			IEmailService emailService)
		{
			_emailService = emailService;
			_ctConnectorFactory = ctConnectorFactory;
		}
		public async Task Create(Account account)
		{
			try
			{
				if (account.MetaTraderAccountId.HasValue) ConnectMtAccount(account);
				else if (account.CTraderAccountId.HasValue) ConenctCtAccount(account);
				else if (account.FixApiAccountId.HasValue) await ConnectFixAccount(account);
				else if (account.IlyaFastFeedAccountId.HasValue) await ConnectIlyaFastFeedAccount(account);
				else if (account.CqgClientApiAccountId.HasValue) await ConnectCqgClientApiAccount(account);
				else if (account.IbAccountId.HasValue) await ConnectIbAccount(account);
			}
			catch (Exception e)
			{
				Logger.Error($"{account} account FAILED to connect", e);
			}
		}

		private void ConnectMtAccount(Account account)
		{
			void DestinationSetter(string host, int port)
			{
				account.DestinationHost = host;
				account.DestinationPort = port;
			}

			if (!(account.Connector is Mt4Integration.Connector) ||
			    account.Connector.Id != account.MetaTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new Mt4Integration.Connector(_emailService);
			((Mt4Integration.Connector) account.Connector)
				.Connect(new Mt4Integration.AccountInfo()
				{
					DbId = account.MetaTraderAccount.Id,
					Description = account.MetaTraderAccount.Description,
					User = account.MetaTraderAccount.User,
					Password = account.MetaTraderAccount.Password,
					Srv = account.MetaTraderAccount.MetaTraderPlatform.SrvFilePath,
					LocalPortForProxy = account.ProfileProxyId.HasValue ? account.ProfileProxy.LocalPort : (int?) null,
					InstrumentConfigs = account.MetaTraderAccount.InstrumentConfigs?
						.ToDictionary(ic => ic.Symbol, ic => ic.Multiplier ?? 1)
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
						Playground = account.CTraderAccount.CTraderPlatform.Playground,
						Debug = account.CTraderAccount.CTraderPlatform.Debug
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

		private async Task ConnectFixAccount(Account account)
		{
			if (!(account.Connector is FixApiIntegration.Connector) ||
			    account.Connector.Id != account.FixApiAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new FixApiIntegration.Connector(new FixApiIntegration.AccountInfo
				{
					DbId = account.FixApiAccount.Id,
					Description = account.FixApiAccount.Description,
					ConfigPath = account.FixApiAccount.ConfigPath,
				}, _emailService);

			await ((FixApiIntegration.Connector) account.Connector).Connect();
		}

		private async Task ConnectIlyaFastFeedAccount(Account account)
		{
			if (!(account.Connector is IlyaFastFeedIntegration.Connector) ||
			    account.Connector.Id != account.IlyaFastFeedAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new IlyaFastFeedIntegration.Connector();

			await ((IlyaFastFeedIntegration.Connector)account.Connector).Connect(new IlyaFastFeedIntegration.AccountInfo()
			{
				DbId = account.IlyaFastFeedAccount.Id,
				Description = account.IlyaFastFeedAccount.Description,
				IpAddress = account.IlyaFastFeedAccount.IpAddress,
				Port = account.IlyaFastFeedAccount.Port,
				UserName = account.IlyaFastFeedAccount.UserName
			});
		}

		private async Task ConnectCqgClientApiAccount(Account account)
		{
			if (!(account.Connector is CqgClientApiIntegration.Connector) ||
			    account.Connector.Id != account.CqgClientApiAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new CqgClientApiIntegration.Connector(new CqgClientApiIntegration.AccountInfo()
				{
					DbId = account.CqgClientApiAccount.Id,
					Description = account.CqgClientApiAccount.Description,
					UserName = account.CqgClientApiAccount.UserName
				});

			await ((CqgClientApiIntegration.Connector)account.Connector).Connect();
		}

		private async Task ConnectIbAccount(Account account)
		{
			if (!(account.Connector is IbIntegration.Connector) ||
			    account.Connector.Id != account.IbAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new IbIntegration.Connector(new IbIntegration.AccountInfo()
				{
					DbId = account.IbAccount.Id,
					Description = account.IbAccount.Description,
					Port = account.IbAccount.Port,
					ClientId = account.IbAccount.ClientId
				});

			await ((IbIntegration.Connector)account.Connector).Connect();
		}
	}
}
