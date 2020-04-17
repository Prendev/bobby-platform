using Autofac;
using log4net;
using TradeSystem.Common;
using TradeSystem.Common.Logging;
using TradeSystem.Duplicat.Views;
using TradeSystem.Orchestration;
using System;
using System.Threading;

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
            RegisterOrchestration(builder);
        }

	    private static void RegisterLoggers()
		{
			Logger.AddLogger(new LogAdapter(LogManager.GetLogger("General")),
				filePathExclude: new[]
				{
					"TradeSystem.Communication"
				});
		}

        private static void RegisterApp(ContainerBuilder builder)
        {
			builder.RegisterType<MainForm>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ViewModel.DuplicatViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.Register((c, p) => new Func<SynchronizationContext>(() => DependecyManager.SynchronizationContext));
		}

        private static void RegisterOrchestration(ContainerBuilder builder)
        {
            builder.RegisterType<Orchestrator>().As<IOrchestrator>();
		}
    }
}
