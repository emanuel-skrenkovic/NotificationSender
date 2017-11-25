using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface IWindowsNotificationService
    {
        void ShowNotification(string title, string text);
    }
}
