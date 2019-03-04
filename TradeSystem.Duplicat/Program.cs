using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using TradeSystem.Data;
using TradeSystem.Duplicat.Views;

namespace TradeSystem.Duplicat
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
		{
			if (bool.TryParse(ConfigurationManager.AppSettings["PrepareAssemblies"], out bool prepareAssemblies) && prepareAssemblies)
				PrepareAssemblies();

			Debug.WriteLine($"Generate ThreadPool threads start at {HiResDatetime.UtcNow:O}");
			int.TryParse(ConfigurationManager.AppSettings["ThreadPool.MinThreads"], out var minThreads);
			ThreadPool.GetMinThreads(out var wokerThreads, out var completionPortThreads);
			var newMinThreads = Math.Max(minThreads, wokerThreads);
			ThreadPool.SetMinThreads(newMinThreads, completionPortThreads);
			Debug.WriteLine($"Generate ThreadPool threads finish at {HiResDatetime.UtcNow:O}");

			using (var c = new DuplicatContext()) c.Init();

			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
            {
				Application.ThreadException += (s, e) => Application_ThreadException(e);
				Application.Run(scope.Resolve<MainForm>());
			}
        }

		private static void Application_ThreadException(ThreadExceptionEventArgs e)
		{
			Logger.Error("Unhandled exception", e.Exception);
		}

	    private static void PrepareAssemblies()
	    {
		    var loadedAssmblies = new HashSet<Assembly>();
		    ForceLoadAll(Assembly.GetExecutingAssembly(), loadedAssmblies);
		    foreach (var assembly in loadedAssmblies) PreJit(assembly);
	    }

	    private static void ForceLoadAll(Assembly assembly, ISet<Assembly> loadedAssmblies)
	    {
		    if (!loadedAssmblies.Add(assembly)) return;

		    foreach (var assemblyName in assembly.GetReferencedAssemblies())
		    {
			    if (assemblyName.Name == "TradeSystem.CTraderApi") continue;
			    if (assemblyName.Name == "TradeSystem.CTraderIntegration") continue;
			    if (assemblyName.Name.Contains("NPOI")) continue;
			    if (assemblyName.Name.Contains("log4net")) continue;

				var nextAssembly = Assembly.Load(assemblyName);
			    if (nextAssembly.GlobalAssemblyCache) continue;

			    ForceLoadAll(nextAssembly, loadedAssmblies);
		    }
	    }

		private static void PreJit(Assembly assembly)
	    {
		    foreach (var type in assembly.GetTypes())
		    {
			    var methods = type.GetMethods(
				    BindingFlags.DeclaredOnly |
				    BindingFlags.NonPublic |
				    BindingFlags.Public |
				    BindingFlags.Instance |
				    BindingFlags.Static);

			    foreach (var method in methods)
			    {
				    if (method.ContainsGenericParameters) continue;
				    if (method.IsAbstract) continue;
				    RuntimeHelpers.PrepareMethod(method.MethodHandle);
				}

			    if (type.IsGenericTypeDefinition || type.IsInterface) continue;
			    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}
	    }
	}
}
