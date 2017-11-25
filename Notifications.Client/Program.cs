using Autofac;
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

            var container = ConfigureDependencyInjection();

            var icon = container.Resolve<IProcessIcon>();

            icon.Display();

            Application.Run();
        }

        private static IContainer ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();
            
            // services

            builder.RegisterType<WindowsNotificationService>()
                .As<IWindowsNotificationService>();

            builder.RegisterType<NetworkService>()
                .As<INetworkService>();

            builder.RegisterType<TcpService>()
                .As<ITcpService>();

            builder.RegisterType<TcpSender>()
                .As<ITcpSender>();

            // windows forms

            builder.RegisterType<ProcessIcon>()
                .As<IProcessIcon>();

            return builder.Build();
        }
    }
}
