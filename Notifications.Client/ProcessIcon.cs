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

        private ITcpServer server;
        private IWindowsNotificationService windowsNotificationService;
        private ITcpService tcpService;

        private IconState iconState = IconState.Off;

        public ProcessIcon(ITcpService tcpService, IWindowsNotificationService windowsNotificationService)
        {
            ni = new NotifyIcon();

            ni.MouseClick += NotifyIcon_MouseClick;

            this.tcpService = tcpService;
            this.windowsNotificationService = windowsNotificationService;
        }

        public void Display()
        {
            if (server == null)
            {
                server = new TcpServer(Host, Port);
                
                if (!server.IsRunning)
                    server.Start(ProcessRequest, null);
            }


            ni.Visible = true;
            ni.Icon = new System.Drawing.Icon(@"D:\downloads\Sora-Meliae-Matrilineare-Mimes-image-x-ico.ico");
        }

        private void ProcessRequest(object res)
        {
            var serializedMsg = res as string;

            var message = JsonConvert.DeserializeObject<NotificationMessage>(serializedMsg);

            iconState = (message.TurnOn) ? IconState.On : IconState.Off;

            if (iconState == IconState.On)
            {
                ni.Icon = new System.Drawing.Icon(@"D:\downloads\Dtafalonso-Android-Lollipop-Youtube.ico");

                var title = message.WindowsNotification.Title;
                var text = message.WindowsNotification.Text;

                windowsNotificationService.ShowNotification(title, text);
            }
            else
            {
                ni.Icon = new System.Drawing.Icon(@"D:\downloads\Sora-Meliae-Matrilineare-Mimes-image-x-ico.ico");
            }

            server.Start(ProcessRequest, null);
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

            ni.MouseClick -= NotifyIcon_MouseClick;
            ni.Dispose();
        }
    }
}
