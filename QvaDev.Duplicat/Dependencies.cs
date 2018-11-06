using System;
using System.Threading;
using Autofac;
using log4net;
using QvaDev.Common;
using QvaDev.Common.Logging;
using QvaDev.Common.Services;
using QvaDev.CTraderIntegration;
using QvaDev.CTraderIntegration.Services;
using QvaDev.Data;
using QvaDev.Data.Repositories;
using QvaDev.Duplicat.Views;
using QvaDev.Orchestration;
using QvaDev.Orchestration.Services;
using QvaDev.Orchestration.Services.Strategies;
using ConnectorFactory = QvaDev.Orchestration.Services.ConnectorFactory;
using ExchangeRatesService = QvaDev.Common.Services.ExchangeRatesService;
using IExchangeRatesService = QvaDev.Common.Services.IExchangeRatesService;

namespace QvaDev.Duplicat
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
		    Logger.AddLogger(new LogAdapter(LogManager.GetLogger("General")), filePathExclude: "QvaDev.Communication");
		    Logger.AddLogger(new LogAdapter(LogManager.GetLogger("FIX")), filePathInclude: "QvaDev.Communication");
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
            builder.RegisterType<ReportService>().As<IReportService>();
			builder.RegisterType<TickerService>().As<ITickerService>();
			builder.RegisterType<HubArbService>().As<IHubArbService>();
			builder.RegisterType<MtAccountImportService>().As<IMtAccountImportService>();
			builder.RegisterType<ProxyService>().As<IProxyService>();
		}
    }
}
