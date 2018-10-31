using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using log4net;
using QvaDev.Data;
using QvaDev.Duplicat.Views;

namespace QvaDev.Duplicat
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
	        int.TryParse(ConfigurationManager.AppSettings["ThreadPool.MinThreads"], out var minThreads);
	        ThreadPool.GetMinThreads(out var wokerThreads, out var completionPortThreads);
	        ThreadPool.SetMinThreads(Math.Max(minThreads, wokerThreads), completionPortThreads);

			using (var c = new DuplicatContext()) c.Init();

			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
            {
	            var log = scope.Resolve<ILog>();

				Application.ThreadException += (s, e) => Application_ThreadException(e, log);
				Application.Run(scope.Resolve<MainForm>());
			}
        }

		private static void Application_ThreadException(ThreadExceptionEventArgs e, ILog log)
		{
			log.Error("Unhandled exception", e.Exception);
		}
	}
}
