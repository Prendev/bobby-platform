using Autofac;
using log4net;
using QvaDev.Configuration.Services;
using QvaDev.CTraderIntegration;
using QvaDev.CTraderIntegration.Services;
using QvaDev.Data;
using QvaDev.Data.Repositories;
using QvaDev.Orchestration;

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
            RegisterConfiguration(builder);
            RegisterData(builder);
            RegisterOrchestration(builder);
        }

        private static void RegisterApp(ContainerBuilder builder)
        {
            builder.RegisterInstance(LogManager.GetLogger(""));
            builder.RegisterType<MainForm>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.DuplicatViewModel>().AsSelf().InstancePerLifetimeScope();
        }

        private static void RegisterConfiguration(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigService>().As<IConfigService>();
            builder.Register((c, p) =>
            {
                var service = c.Resolve<IConfigService>();
                return service.Config;
            }).InstancePerLifetimeScope();
        }

        private static void RegisterData(ContainerBuilder builder)
        {
            builder.RegisterType<MetaTraderPlatformRepository>().As<IMetaTraderPlatformRepository>();
        }

        private static void RegisterOrchestration(ContainerBuilder builder)
        {
            builder.RegisterType<Orchestrator>().As<IOrchestrator>();
            builder.RegisterType<ConnectorFactory>().As<IConnectorFactory>();
            builder.RegisterType<TradingAccountsService>().As<ITradingAccountsService>();
            builder.RegisterType<RestService>().As<IRestService>();
        }
    }
}
