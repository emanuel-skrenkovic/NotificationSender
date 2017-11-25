using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface ITcpService
    {
        void Send(string message, string host, int port);

        void Send<TMessage>(TMessage messageObj, string host, int port);

        Task SendAsync(string message, string host, int port);

        Task SendAsync<TMessage>(TMessage messageObj, string host, int port);
    }
}
