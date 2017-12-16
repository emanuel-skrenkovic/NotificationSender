using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface ISenderService
    {
        void Send(string message, string host, int port);

        void Send<TMessage>(TMessage messageObj, string host, int port);

        Task SendAsync(string message, string host, int port);

        Task SendAsync<TMessage>(TMessage messageObj, string host, int port);

        void SendBatch(string message, List<string> hosts, int port);

        void SendBatch<TMessage>(TMessage messageObj, List<string> hosts, int port);

        Task SendBatchAsync(string message, List<string> hosts, int port);

        Task SendBatchAsync<TMessage>(TMessage messageObj, List<string> hosts, int port);
    }
}
