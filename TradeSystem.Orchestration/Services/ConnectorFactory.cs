using mtapi.mt5;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading.Tasks;
using TradeSystem.Common.Services;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Notification.Services;
using Mt4PlacedType = TradingAPI.MT4Server.PlacedType;
using Mt5PlacedType = mtapi.mt5.PlacedType;

namespace TradeSystem.Orchestration.Services
{
	public class ConnectorFactory : IConnectorFactory
	{
		private readonly CTraderIntegration.ICtConnectorFactory _ctConnectorFactory;
		private readonly IEmailService _emailService;
		private readonly ITelegramService _telegramService;
		private readonly ITwilioService _twilioService;

		public ConnectorFactory(
			CTraderIntegration.ICtConnectorFactory ctConnectorFactory,
			IEmailService emailService,
			ITelegramService telegramService,
			ITwilioService twilioService)
		{
			_emailService = emailService;
			_ctConnectorFactory = ctConnectorFactory;
			_telegramService = telegramService;
			_twilioService = twilioService;
		}
		public async Task Create(Account account)
		{
			try
			{
				if (account.MetaTraderAccountId.HasValue && account.Nj4x) ConnectMtNj4xAccount(account);
				else if (account.MetaTraderAccountId.HasValue) ConnectMtAccount(account);
				else if (account.CTraderAccountId.HasValue) ConenctCtAccount(account);
				else if (account.FixApiAccountId.HasValue) await ConnectFixAccount(account);
				else if (account.CqgClientApiAccountId.HasValue) await ConnectCqgClientApiAccount(account);
				else if (account.IbAccountId.HasValue) await ConnectIbAccount(account);
				else if (account.BacktesterAccountId.HasValue) ConnectBtAccount(account);
				else return;
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
			((Mt4Integration.Connector)account.Connector)
				.Connect(new Mt4Integration.AccountInfo()
				{
					DbId = account.MetaTraderAccount.Id,
					Description = account.MetaTraderAccount.Description,
					User = account.MetaTraderAccount.User,
					Password = account.MetaTraderAccount.Password,
					Srv = account.MetaTraderAccount.MetaTraderPlatform.SrvFilePath,
					BackupSrv = account.MetaTraderAccount.MetaTraderPlatform.BackupSrvFilePath,
					InstrumentConfigs = account.MetaTraderAccount.InstrumentConfigs?
						.ToDictionary(ic => ic.Symbol, ic => ic.Multiplier ?? 1),
					ProxyEnable = account.MetaTraderAccount.ProxyEnable,
					ProxyHost = account.MetaTraderAccount.ProxyHost,
					ProxyPort = account.MetaTraderAccount.ProxyPort,
					ProxyType = account.MetaTraderAccount.ProxyType == Proxy.ProxyTypes.Socks5 ?
					TradingAPI.MT4Server.ProxyTypes.Socks5 : account.MetaTraderAccount.ProxyType == Proxy.ProxyTypes.Socks4
					? TradingAPI.MT4Server.ProxyTypes.Socks4 : TradingAPI.MT4Server.ProxyTypes.Https,
					ProxyUser = account.MetaTraderAccount.ProxyUser ?? string.Empty,
					ProxyPassword = account.MetaTraderAccount.ProxyPassword ?? string.Empty,
					PlacedType = (Mt4PlacedType)account.MetaTraderAccount.PlacedType
				}, DestinationSetter);
		}
		private void ConnectMtNj4xAccount(Account account)
		{
			void DestinationSetter(string host, int port)
			{
				account.DestinationHost = host;
				account.DestinationPort = port;
			}

			if (!(account.Connector is Nj4xIntegration.Connector) ||
				account.Connector.Id != account.MetaTraderAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new Nj4xIntegration.Connector(_emailService);
			((Nj4xIntegration.Connector)account.Connector)
				.Connect(new Nj4xIntegration.AccountInfo()
				{
					DbId = account.MetaTraderAccount.Id,
					Description = account.MetaTraderAccount.Description,
					User = account.MetaTraderAccount.User,
					Password = account.MetaTraderAccount.Password,
					Srv = account.MetaTraderAccount.MetaTraderPlatform.SrvFilePath,
					BackupSrv = account.MetaTraderAccount.MetaTraderPlatform.BackupSrvFilePath,
					InstrumentConfigs = account.MetaTraderAccount.InstrumentConfigs?
						.ToDictionary(ic => ic.Symbol, ic => ic.Multiplier ?? 1),
					ProxyEnable = account.MetaTraderAccount.ProxyEnable,
					ProxyHost = account.MetaTraderAccount.ProxyHost,
					ProxyPort = account.MetaTraderAccount.ProxyPort,
					ProxyType = account.MetaTraderAccount.ProxyType == Proxy.ProxyTypes.Socks5 ?
					nj4x.Metatrader.Broker.ProxyType.SOCKS5 : account.MetaTraderAccount.ProxyType == Proxy.ProxyTypes.Socks4
					? nj4x.Metatrader.Broker.ProxyType.SOCKS4 : nj4x.Metatrader.Broker.ProxyType.HTTP,
					ProxyUser = account.MetaTraderAccount.ProxyUser ?? string.Empty,
					ProxyPassword = account.MetaTraderAccount.ProxyPassword ?? string.Empty,

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
					PlacedType = (Mt5PlacedType)account.FixApiAccount.PlacedType
				}, _emailService);

			await ((FixApiIntegration.Connector)account.Connector).Connect();
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

		private void ConnectBtAccount(Account account)
		{
			if (!(account.Connector is Backtester.Connector) ||
				account.Connector.Id != account.BacktesterAccountId)
				account.Connector = null;

			if (account.Connector == null)
				account.Connector = new Backtester.Connector(account.BacktesterAccount);
			((Backtester.Connector)account.Connector).Connect();
		}
	}
}
