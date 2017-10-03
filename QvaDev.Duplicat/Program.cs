﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var scope = Dependencies.GetContainer().BeginLifetimeScope())
            {
                Application.Run(scope.Resolve<MainForm>());
            }
        }
    }
}
