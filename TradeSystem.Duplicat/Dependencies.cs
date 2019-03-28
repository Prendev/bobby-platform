using Autofac;
using log4net;
using TradeSystem.Common;
using TradeSystem.Common.Logging;
using TradeSystem.Common.Services;
using TradeSystem.CTraderIntegration;
using TradeSystem.CTraderIntegration.Services;
using TradeSystem.Data;
using TradeSystem.Data.Repositories;
using TradeSystem.Duplicat.Views;
using TradeSystem.Orchestration;
using TradeSystem.Orchestration.Services;
using TradeSystem.Orchestration.Services.Strategies;
using System;
using System.Threading;
using ConnectorFactory = TradeSystem.Orchestration.Services.ConnectorFactory;
using ExchangeRatesService = TradeSystem.Common.Services.ExchangeRatesService;
using IExchangeRatesService = TradeSystem.Common.Services.IExchangeRatesService;

namespace TradeSystem.Duplicat
{
	public class Dependencies
    {
		public static IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            Register(builder);
            return builder.Build();
        }

        private static void Register(ContainerBuilder builder)
        {
	        RegisterLoggers();
			RegisterApp(builder);
	        RegisterCommon(builder);
            RegisterData(builder);
            RegisterOrchestration(builder);
        }

	    private static void RegisterLoggers()
	    {
		    Logger.AddLogger(new LogAdapter(LogManager.GetLogger("General")),
			    filePathExclude: new[] {"TradeSystem.Communication" });
		    Logger.AddLogger(new LogAdapter(LogManager.GetLogger("FIX")),
			    filePathInclude: new[] { "TradeSystem.Communication" });
		    Logger.AddLogger(new LogAdapter(LogManager.GetLogger("FIX copy")),
			    filePathInclude: new[] { @"TradeSystem.Orchestration\Services\CopyLogger" });
			Logger.AddLogger(new LogAdapter(LogManager.GetLogger("FIX orders")),
				filePathInclude: new[] { @"TradeSystem.FixApiIntegration\FillLogger" });
		}

        private static void RegisterApp(ContainerBuilder builder)
        {
			builder.RegisterType<MainForm>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.DuplicatViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.Register((c, p) => new Func<SynchronizationContext>(() => DependecyManager.SynchronizationContext));
		}

	    private static void RegisterCommon(ContainerBuilder builder)
		{
			builder.RegisterType<RndService>().As<IRndService>();
			builder.RegisterType<ThreadService>().As<IThreadService>();
			builder.RegisterType<XmlService>().As<IXmlService>();
		    builder.RegisterType<ExchangeRatesService>().As<IExchangeRatesService>();
		    builder.RegisterType<NewsCalendarService>().As<INewsCalendarService>();
		    builder.RegisterType<EmailService>().As<IEmailService>();
		}

		private static void RegisterData(ContainerBuilder builder)
        {
            builder.RegisterType<MetaTraderPlatformRepository>().As<IMetaTraderPlatformRepository>();
            builder.RegisterType<XmlService>().As<IXmlService>();
        }

        private static void RegisterOrchestration(ContainerBuilder builder)
        {
            builder.RegisterType<Orchestrator>().As<IOrchestrator>();
            builder.RegisterType<CtConnectorFactory>().As<ICtConnectorFactory>();
	        builder.RegisterType<ConnectorFactory>().As<IConnectorFactory>();
			builder.RegisterType<TradingAccountsService>().As<ITradingAccountsService>();
            builder.RegisterType<RestService>().As<IRestService>();
            builder.RegisterType<CopierService>().As<ICopierService>();
	        builder.RegisterType<PushingService>().As<IPushingService>();
	        builder.RegisterType<SpoofingService>().As<ISpoofingService>();
	        builder.RegisterType<TwoWaySpoofingService>().As<ITwoWaySpoofingService>();
			builder.RegisterType<PushStrategyService>().As<IPushStrategyService>();
			builder.RegisterType<SpoofStrategyService>().As<ISpoofStrategyService>();
            builder.RegisterType<ReportService>().As<IReportService>();
			builder.RegisterType<TickerService>().As<ITickerService>();
			builder.RegisterType<HubArbService>().As<IHubArbService>();
			builder.RegisterType<MarketMakerService>().As<IMarketMakerService>();
			builder.RegisterType<MtAccountImportService>().As<IMtAccountImportService>();
			builder.RegisterType<ProxyService>().As<IProxyService>();
		}
    }
}
