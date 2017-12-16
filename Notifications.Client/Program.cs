using Autofac;
using Notifications.Common;
using Notifications.Service;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notifications.Client
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
            //Application.Run(new Form1());

            var container = AppStart.GetDIContainer();

            var icon = container.Resolve<IProcessIcon>();

            icon.Display();

            var pingListener = new PingListener(container.Resolve<IServer>(), container.Resolve<ISenderService>(), container.Resolve<INetworkService>(), container.Resolve<UtilityService>());
            pingListener.DoWork += pingListener.StartListen;

            pingListener.RunWorkerAsync();

            Application.Run();
        }
    }
}
