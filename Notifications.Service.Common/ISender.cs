using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface ISender
    {
        void Send(string message, string host, int port);

        Task SendAsync(string message, string host, int port);
    }
}
