using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Models
{
    public class NotificationMessage
    {
        public bool TurnOn { get; set; }

        public WindowsNotification WindowsNotification { get; set; }
    }
}
