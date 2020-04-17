using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
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
        [HandleProcessCorruptedStateExceptions]
		static void Main()
        {
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Directory.CreateDirectory("FileContext");

				using (var c = new DuplicatContext()) c.Init();
				using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
				{
					Application.ThreadException += (s, e) => Application_ThreadException(e);
					Application.Run(scope.Resolve<MainForm>());

				}
			}
			catch (Exception e)
			{
				Logger.Error("Unhandled exception", e);
				throw;
			}
        }

		private static void Application_ThreadException(ThreadExceptionEventArgs e)
		{
			Logger.Error("Unhandled exception", e.Exception);
		}
	}
}
