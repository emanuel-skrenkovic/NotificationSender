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
        private IClient client;

        public HttpService(IClient client)
        {
            this.client = client;
        }

        public TResponse Send<TResponse>(string message, string host, int port) where TResponse : class
        {
            return client.Send<TResponse>(message, host, port);
        }

        public TResponse Send<TMessage, TResponse>(TMessage messageObj, string host, int port)
            where TMessage : class
            where TResponse : class
        {
            string serializedMessage = JsonConvert.SerializeObject(messageObj);

            Test.Service.Http.IClient test2 = new Test.Service.Http.HttpClient() as Test.Service.Http.IClient;
            //var test = new Test.Service.Http.HttpClient();
            return test2.Send<TResponse>(serializedMessage, host, port);

            //return client.Send<TResponse>(serializedMessage, host, port);
        }

        public Task<TResponse> SendAsync<TResponse>(string message, string host, int port) where TResponse : class
        {
            return client.SendAsync<TResponse>(message, host, port);
        }

        public Task<TResponse> SendAsync<TMessage, TResponse>(TMessage messageObj, string host, int port)
            where TMessage : class
            where TResponse : class
        {
            string serializedMessage = JsonConvert.SerializeObject(messageObj);

            return client.SendAsync<TResponse>(serializedMessage, host, port);
        }

        public List<TResponse> SendBatch<TResponse>(string message, List<string> hosts, int port) where TResponse : class
        {
            var result = new List<TResponse>();

            foreach (var host in hosts)
            {
                result.Add(Send<TResponse>(message, host, port));
            }

            return result;
        }

        public List<TResponse> SendBatch<TMessage, TResponse>(TMessage messageObj, List<string> hosts, int port)
            where TMessage : class
            where TResponse : class
        {
            var result = new List<TResponse>();

            foreach (var host in hosts)
            {
                result.Add(Send<TMessage, TResponse>(messageObj, host, port));
            }

            return result;
        }

        public async Task<List<TResponse>> SendBatchAsync<TResponse>(string message, List<string> hosts, int port) where TResponse : class
        {
            var taskList = new List<Task<TResponse>>();

            foreach (var host in hosts)
            {
                taskList.Add(SendAsync<TResponse>(message, host, port));
            }

            var result = await Task.WhenAll(taskList);

            return result.ToList();
        }

        public async Task<List<TResponse>> SendBatchAsync<TMessage, TResponse>(TMessage messageObj, List<string> hosts, int port)
            where TMessage : class
            where TResponse : class
        {
            var taskList = new List<Task<TResponse>>();

            foreach (var host in hosts)
            {
                taskList.Add(SendAsync<TMessage, TResponse>(messageObj, host, port));
            }

            var result = await Task.WhenAll(taskList);

            return result.ToList();
        }
    }
}
