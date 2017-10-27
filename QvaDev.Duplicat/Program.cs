using System;
using System.IO;
using System.Windows.Forms;
using Autofac;
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
            using (var c = new DuplicatContext())
                c.Init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
            {
                Application.Run(scope.Resolve<MainForm>());
            }
        }
    }
}
