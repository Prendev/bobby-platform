using log4net;
using QvaDev.CTraderIntegration;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration.Services
{
	public class ConnectorFactory : IConnectorFactory
	{
		private readonly ICtConnectorFactory _ctConnectorFactory;
		private readonly ILog _log;

		public ConnectorFactory(
			ICtConnectorFactory ctConnectorFactory,
			ILog log)
		{
			_log = log;
			_ctConnectorFactory = ctConnectorFactory;
		}
		public void Create(Account account)
		{
			if (account.MetaTraderAccountId.HasValue) ConnectMtAccount(account);
			if (account.CTraderAccountId.HasValue) ConenctCtAccount(account);
			if (account.FixTraderAccountId.HasValue) ConnectFtAccount(account);
			if (account.FixApiAccountId.HasValue) ConnectFixAccount(account);
		}


		private void ConnectMtAccount(Account account)
		{
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
				});
		}

		private void ConenctCtAccount(Account account)
		{
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
			if (account.Connector == null)
				account.Connector = new FixTraderIntegration.Connector(_log);

			((FixTraderIntegration.Connector) account.Connector)
				.Connect(new FixTraderIntegration.AccountInfo
				{
					Description = account.FixTraderAccount.Description,
					IpAddress = account.FixTraderAccount.IpAddress,
					CommandSocketPort = account.FixTraderAccount.CommandSocketPort,
					EventsSocketPort = account.FixTraderAccount.EventsSocketPort
				});
		}

		private void ConnectFixAccount(Account account)
		{
			if (account.Connector == null)
				account.Connector = new FixApiIntegration.Connector();

			((FixApiIntegration.Connector)account.Connector)
				.Connect();
		}
	}
}
