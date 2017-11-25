using Newtonsoft.Json;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class TcpService : ITcpService
    {
        public TcpService() { }

        public void Send(string message, string host, int port)
        {
            SendInternal(message, host, port);
        }
        
        public void Send<TMessage>(TMessage messageObj, string host, int port)
        {
            string serializedMsg = JsonConvert.SerializeObject(messageObj);

            SendInternal(serializedMsg, host, port);
        }

        public Task SendAsync(string message, string host, int port)
        {
            return SendInternalAsync(message, host, port);
        }

        public Task SendAsync<TMessage>(TMessage messageObj, string host, int port)
        {
            string serializedMsg = JsonConvert.SerializeObject(messageObj);

            return SendInternalAsync(serializedMsg, host, port);
        }

        private void SendInternal(string message, string host, int port)
        {
            using (var client = new TcpSender())
            {
                client.Send(message, host, port);
            }
        }

        private Task SendInternalAsync(string message, string host, int port)
        {
            using (var client = new TcpSender())
            {
                return client.SendAsync(message, host, port);
            }
        }
    }
}
