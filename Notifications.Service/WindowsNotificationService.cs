using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Notifications.Service
{
    public class WindowsNotificationService : IWindowsNotificationService
    {
        public void ShowNotification(string title, string text)
        {
            string xmlString = CreateToastXml(title, text);

            var doc = new XmlDocument();
            doc.LoadXml(xmlString);

            var toast = new ToastNotification(doc);

            ToastNotificationManager.CreateToastNotifier("Toast").Show(toast);
        }

        private string CreateToastXml(string title, string text)
        {
            string xml = $@"
                <toast>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>{title}</text>
                            <text>{text}</text>
                        </binding>
                    </visual>
                </toast>";

            return xml;
        }
    }
}
