using Notifications.Client.Resources;
using Notifications.Common;
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
        private NotifyIcon ni;

        private System.Drawing.Icon offIcon;
        private System.Drawing.Icon onIcon;

        private IServer server;
        private ISenderService senderService;
        private INetworkService networkService;
        private IWindowsNotificationService windowsNotificationService;
        private UtilityService utility;

        private IconState iconState = IconState.Off;

        public ProcessIcon(IServer server, ISenderService senderService, INetworkService networkService, IWindowsNotificationService windowsNotificationService, UtilityService utility)
        {
            ni = new NotifyIcon();

            ni.MouseClick += NotifyIcon_MouseClick;

            this.server = server;
            this.senderService = senderService;
            this.networkService = networkService;
            this.windowsNotificationService = windowsNotificationService;
            this.utility = utility;

            offIcon = new System.Drawing.Icon(Images.GreenIcon);
            onIcon = new System.Drawing.Icon(Images.RedIcon);
        }

        public void Display()
        {
            ni.Visible = true;
            ni.Icon = offIcon;

            if (server != null && !server.IsRunning)
                server.Start(Routes.NotifyRoute, ProcessRequest, null);
        }

        private void ProcessRequest(object res, object state)
        {
            var message = utility.DeserializeFromObject<NotificationMessage>(res);

            if (message == null)
                return;

            iconState = (message.TurnOn) ? IconState.On : IconState.Off;

            if (iconState == IconState.On)
            {
                ni.Icon = onIcon;

                var title = message.WindowsNotification.Title;
                var text = message.WindowsNotification.Text;

                windowsNotificationService.ShowNotification(title, text);
            }
            else
            {
                ni.Icon = offIcon;
            }
        }

        private async void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                try
                {
                    var message = CreateNotificationMessage();

                    var availableIps = await networkService.GetAvailableNetworkPcsAsync();
                    var addressList = availableIps.Select(ip => $"{ip}{Routes.NotifyRoute}").ToList();

                    await senderService.SendBatchAsync(message, addressList, NetworkConsts.TCP_PORT);
                } 
                catch (Exception ex)
                {
                    DisplayError();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                try
                {
                    var message = CreatePingRequest();

                    var availableIps = await networkService.GetAvailableNetworkPcsAsync();
                    var addressList = availableIps.Select(ip => $"{ip}{Routes.PingRoute}").ToList();

                    await senderService.SendBatchAsync(message, addressList, NetworkConsts.TCP_PORT);
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

        private PingRequest CreatePingRequest()
        {
            var pingRequest = new PingRequest
            {
                RequestorAddress = networkService.GetIpString(),
            };

            return pingRequest;
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
            //server.Stop();

            offIcon.Dispose();
            onIcon.Dispose();

            ni.MouseClick -= NotifyIcon_MouseClick;
            ni.Dispose();
        }
    }
}
