using Newtonsoft.Json;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class HttpService : ISenderService
    {
        private ISender client;

        public HttpService(ISender client)
        {
            this.client = client;           
        }

        public void Send(string message, string host, int port)
        {
            client.Send(message, host, port);
        }

        public void Send<TMessage>(TMessage messageObj, string host, int port)
        {
            string serializedMessage = JsonConvert.SerializeObject(messageObj);

            client.Send(serializedMessage, host, port);
        }

        public Task SendAsync(string message, string host, int port)
        {
            return client.SendAsync(message, host, port);
        }

        public Task SendAsync<TMessage>(TMessage messageObj, string host, int port)
        {
            string serializedMessage = JsonConvert.SerializeObject(messageObj);

            return client.SendAsync(serializedMessage, host, port);
        }

        public void SendBatch(string message, List<string> hosts, int port)
        {
            foreach (var host in hosts)
            {
                Send(message, host, port);
            }
        }

        public void SendBatch<TMessage>(TMessage messageObj, List<string> hosts, int port)
        {
            foreach (var host in hosts)
            {
                Send(messageObj, host, port);
            }
        }

        public Task SendBatchAsync(string message, List<string> hosts, int port)
        {
            var taskList = new List<Task>();

            foreach (var host in hosts)
            {
                taskList.Add(SendAsync(message, host, port));
            }

            return Task.WhenAll(taskList);
        }

        public Task SendBatchAsync<TMessage>(TMessage messageObj, List<string> hosts, int port)
        {
            var taskList = new List<Task>();

            foreach (var host in hosts)
            {
                taskList.Add(SendAsync(messageObj, host, port));
            }

            return Task.WhenAll(taskList);
        }
    }
}
