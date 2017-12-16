using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class HttpSender : ISender
    {
        private HttpClient client;

        public HttpSender()
        {
            client = new HttpClient();
        }

        public void Send(string message, string host, int port)
        {
            var request = CreateRequestMessage(message, host, HttpMethod.Post);

            var result = client.SendAsync(request).Result;
        }

        public Task SendAsync(string message, string host, int port)
        {
            var request = CreateRequestMessage(message, host, HttpMethod.Post);

            return client.SendAsync(request);
        }

        private HttpRequestMessage CreateRequestMessage(string message, string requestUri, HttpMethod httpMethod)
        {
            var request = new HttpRequestMessage();

            request.Content = new StringContent(message);
            request.RequestUri = new Uri($"http://{requestUri}");
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            request.Method = httpMethod;

            return request;
        }
    }
}
