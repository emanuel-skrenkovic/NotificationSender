using Newtonsoft.Json;
using Notifications.Models;
using Notifications.Service;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notifications.Client
{
    public enum IconState
    {
        On,
        Off
    }

    public class ProcessIcon : IProcessIcon, IDisposable
    {
        private readonly string Host = "192.168.5.10";
        private readonly int Port = 14769;

        private NotifyIcon ni;

        private System.Drawing.Icon offIcon;
        private System.Drawing.Icon onIcon;

        private ITcpServer server;
        private ITcpService tcpService;
        private INetworkService networkService;
        private IWindowsNotificationService windowsNotificationService;

        private IconState iconState = IconState.Off;

        public ProcessIcon(ITcpService tcpService, INetworkService networkService, IWindowsNotificationService windowsNotificationService)
        {
            ni = new NotifyIcon();

            ni.MouseClick += NotifyIcon_MouseClick;

            this.tcpService = tcpService;
            this.networkService = networkService;
            this.windowsNotificationService = windowsNotificationService;

            offIcon = new System.Drawing.Icon(@"D:\downloads\Sora-Meliae-Matrilineare-Mimes-image-x-ico.ico");
            onIcon = new System.Drawing.Icon(@"D:\downloads\Dtafalonso-Android-Lollipop-Youtube.ico");
        }

        public void Display()
        {
            if (server == null)
            {
                server = new TcpServer(Host, Port);
                
                if (!server.IsRunning)
                    server.Start(ProcessPingRequest, new ClientsList { AvailableClients = new List<string>() });
            }

            ni.Visible = true;
            ni.Icon = offIcon;
        }

        private void ProcessRequest(object res, object state)
        {
            var serializedMsg = res as string;

            if (serializedMsg == null)
                return;

            var message = JsonConvert.DeserializeObject<NotificationMessage>(serializedMsg);

            iconState = (message.TurnOn) ? IconState.On : IconState.Off;

            if (iconState == IconState.On)
            {
                ni.Icon = onIcon;

                var title = message.WindowsNotification.Title;
                var text = message.WindowsNotification.Text;

                windowsNotificationService.ShowNotification("test", "test2");
            }
            else
            {
                ni.Icon = offIcon;
            }
        }

        private void ProcessPingRequest(object res, object state)
        {
            var serializedMsg = res as string;

            if (serializedMsg == null || serializedMsg == "ping response")
                return;

            var message = JsonConvert.DeserializeObject<PingRequest>(serializedMsg);

            var clientsList = state as ClientsList;

            clientsList.AvailableClients.Add(Host);

            var response = new PingResponse
            {
                Address = networkService.GetIpString(),
            };

            tcpService.Send(response, Host, Port);
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    var message = CreateNotificationMessage();

                    tcpService.Send(message, Host, Port);
                } 
                catch (Exception ex)
                {
                    DisplayError();
                }
            }

            if (e.Button == MouseButtons.Middle)
            {
                try
                {
                    var message = new PingRequest
                    {
                        RequestorAddress = networkService.GetIpString(),
                    };

                    var availableIps = networkService.GetClients("192.168.5");

                    tcpService.SendBatchAsync(message, availableIps, Port);
                }
                catch (Exception ex)
                {
                    DisplayError();
                }
            }
        }

        private NotificationMessage CreateNotificationMessage()
        {
            var windowsNotification = new WindowsNotification
            {
                Title = "Notification",
                Text = "Sent",
            };

            var message = new NotificationMessage
            {
                TurnOn = true,
                WindowsNotification = windowsNotification,
            };

            return message;
        }

        private void DisplayError()
        {
            ni.BalloonTipIcon = ToolTipIcon.Info;
            ni.BalloonTipText = "Error sending request";
            ni.BalloonTipTitle = "Something went wrong";

            ni.ShowBalloonTip(1000);
        }

        public void Dispose()
        {
            server.Stop();

            offIcon.Dispose();
            onIcon.Dispose();

            ni.MouseClick -= NotifyIcon_MouseClick;
            ni.Dispose();
        }
    }
}
