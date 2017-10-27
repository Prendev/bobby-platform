using System;
using System.Threading;
using Autofac;
using log4net;
using QvaDev.Common.Services;
using QvaDev.CTraderIntegration;
using QvaDev.CTraderIntegration.Services;
using QvaDev.Data.Repositories;
using QvaDev.Duplicat.Views;
using QvaDev.Orchestration;
using QvaDev.Orchestration.Services;
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
            RegisterData(builder);
            RegisterOrchestration(builder);
        }

        private static void RegisterApp(ContainerBuilder builder)
        {
            builder.RegisterInstance(LogManager.GetLogger(""));
            builder.RegisterType<MainForm>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.DuplicatViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.Register((c, p) => new Func<SynchronizationContext>(() => SynchronizationContext.Current));
        }

        private static void RegisterData(ContainerBuilder builder)
        {
            builder.RegisterType<MetaTraderPlatformRepository>().As<IMetaTraderPlatformRepository>();
            builder.RegisterType<XmlService>().As<IXmlService>();
        }

        private static void RegisterOrchestration(ContainerBuilder builder)
        {
            builder.RegisterType<Orchestrator>().As<IOrchestrator>();
            builder.RegisterType<ConnectorFactory>().As<IConnectorFactory>();
            builder.RegisterType<TradingAccountsService>().As<ITradingAccountsService>();
            builder.RegisterType<RestService>().As<IRestService>();
            builder.RegisterType<ExchangeRatesService>().As<IExchangeRatesService>();
            builder.RegisterType<BalanceReportService>().As<IBalanceReportService>();
            builder.RegisterType<CopierService>().As<ICopierService>();
            builder.RegisterType<MonitorServices>().As<IMonitorServices>();
        }
    }
}
