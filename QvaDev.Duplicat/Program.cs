using System;
using System.IO;
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
			AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory));
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

		private static void Application_ThreadException(System.Threading.ThreadExceptionEventArgs e, ILog log)
		{
			log.Error("Unhandled exception", e.Exception);
		}
	}
}
