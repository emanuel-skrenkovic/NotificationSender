using Autofac;
using Notifications.Service;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Client
{
    public static class AppStart
    {
        public static IContainer GetDIContainer()
        {
            var builder = new ContainerBuilder();

            // services

            RegisterServiceTypes(builder);

            // windows forms

            RegisterFormsTypes(builder);

            return builder.Build();
        }

        private static void RegisterServiceTypes(ContainerBuilder builder)
        {
            builder.RegisterType<WindowsNotificationService>()
               .As<IWindowsNotificationService>();

            builder.RegisterType<NetworkService>()
                .As<INetworkService>();

            builder.RegisterType<TcpService>()
                .As<ITcpService>();

            builder.RegisterType<TcpSender>()
                .As<ITcpSender>();
        }

        private static void RegisterFormsTypes(ContainerBuilder builder)
        {
            builder.RegisterType<ProcessIcon>()
                .As<IProcessIcon>();
        }
    }
}
