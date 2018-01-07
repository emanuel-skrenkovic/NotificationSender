using Newtonsoft.Json;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class HttpClient : IClient
    {
        private System.Net.Http.HttpClient client;

        public HttpClient()
        {
            client = new System.Net.Http.HttpClient();
        }

        public TResponse Send<TResponse>(string message, string host, int port) where TResponse : class
        {
            var request = CreateRequestMessage(message, host, HttpMethod.Post);

            var httpResponse = client.SendAsync(request).Result;

            var result = GetResponseObjAsync<TResponse>(httpResponse).Result;

            return result;
        }

        public async Task<TResponse> SendAsync<TResponse>(string message, string host, int port) where TResponse : class
        {
            var request = CreateRequestMessage(message, host, HttpMethod.Post);

            var httpResponse = await client.SendAsync(request);

            var result = await GetResponseObjAsync<TResponse>(httpResponse);

            return result;
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

        private async Task<TResponse> GetResponseObjAsync<TResponse>(HttpResponseMessage httpResponse) where TResponse : class
        {
            var content = httpResponse.Content;
            var serializedResponse = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(serializedResponse);
        }
    }
}
