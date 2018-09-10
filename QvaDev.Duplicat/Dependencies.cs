using System;
using System.Threading;
using Autofac;
using QvaDev.Common;
using QvaDev.Common.Logging;
using QvaDev.Common.Services;
using QvaDev.Communication;
using QvaDev.CTraderIntegration;
using QvaDev.CTraderIntegration.Services;
using QvaDev.Data;
using QvaDev.Data.Repositories;
using QvaDev.Duplicat.Views;
using QvaDev.Orchestration;
using QvaDev.Orchestration.Services;
using QvaDev.Orchestration.Services.Strategies;
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
            RegisterApp(builder);
	        RegisterCommon(builder);
            RegisterData(builder);
            RegisterOrchestration(builder);
        }

        private static void RegisterApp(ContainerBuilder builder)
        {
	        var generalLog = log4net.LogManager.GetLogger("General");
	        var fixLog = log4net.LogManager.GetLogger("FIX");
			Logger.Instance = new LogAdapter(fixLog);
			builder.RegisterInstance(generalLog);
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
			builder.RegisterType<StrategiesService>().As<IStrategiesService>();
			builder.RegisterType<HubArbService>().As<IHubArbService>();
			builder.RegisterType<MtAccountImportService>().As<IMtAccountImportService>();
		}
    }
}
