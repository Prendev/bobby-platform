using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using TradeSystem.Common.Services;
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
        [HandleProcessCorruptedStateExceptions]
		static void Main()
        {
			IEmailService emailService = null;
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				Directory.CreateDirectory("FileContext");
				Directory.CreateDirectory("Mt4SrvFiles");
				Directory.CreateDirectory("FixApiConfigFiles");
				Directory.CreateDirectory("Tickers");

				Debug.WriteLine($"Generate ThreadPool threads start at {HiResDatetime.UtcNow:O}");
				int.TryParse(ConfigurationManager.AppSettings["ThreadPool.MinThreads"], out var minThreads);
				ThreadPool.GetMinThreads(out var wokerThreads, out var completionPortThreads);
				var newMinThreads = Math.Max(minThreads, wokerThreads);
				ThreadPool.SetMinThreads(newMinThreads, completionPortThreads);
				Debug.WriteLine($"Generate ThreadPool threads finish at {HiResDatetime.UtcNow:O}");

				using (var c = new DuplicatContext()) c.Init();
				using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
				{
					emailService = scope.Resolve<IEmailService>();
					Application.ThreadException += (s, e) => Application_ThreadException(e);
					Application.Run(scope.Resolve<MainForm>());

				}
			}
			catch (Exception e)
			{
				Logger.Error("Unhandled exception", e);
				emailService?.Send("Unhandled exception", e.ToString());
				throw;
			}
        }

		private static void Application_ThreadException(ThreadExceptionEventArgs e)
		{
			Logger.Error("Unhandled exception", e.Exception);
		}
	}
}
