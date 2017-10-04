using Autofac;
using log4net;
using QvaDev.Configuration.Services;
using QvaDev.Data;
using QvaDev.Data.Repositories;

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
        }

        private static void RegisterApp(ContainerBuilder builder)
        {
            builder.RegisterInstance(LogManager.GetLogger(""));
            builder.RegisterType<MainForm>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.DuplicatViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.SaveCommand>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.SwitchProfileCommand>().AsSelf().InstancePerLifetimeScope();
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
    }
}
