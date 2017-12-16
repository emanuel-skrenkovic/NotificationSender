using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class HttpServer : IServer
    {
        private HttpListener listener;

        private IController controller;

        private bool isRunning = false;
        private string baseUrl;

        public bool IsRunning { get { return isRunning;  } }

        public HttpServer(string ipAddress)
        {
            listener = new HttpListener();

            controller = new HttpController($"http://{ipAddress}");

            baseUrl = $"http://{ipAddress}";
        }

        public void Start(string route, Action<object, object> callback, object state)
        {
            controller.RegisterRoute(route, callback);
            listener.Prefixes.Add($"{baseUrl}{route}");

            if (isRunning != true)
                listener.Start();

            listener.BeginGetContext(ProcessResult, state);

            isRunning = true;
        }

        public void Stop()
        {
            listener.Stop();

            isRunning = true;
        }

        private void ProcessResult(IAsyncResult res)
        {
            InvokeCallback(res);

            listener.Start();
            listener.BeginGetContext(ProcessResult, res.AsyncState);
        }

        private void InvokeCallback(IAsyncResult res)
        {
            var context = listener.EndGetContext(res);

            var httpMessage = context.Request;
            using (StreamReader inputStream = new StreamReader(httpMessage.InputStream))
            {
                string content = inputStream.ReadToEndAsync().Result;

                controller.HandleRouting(httpMessage.RawUrl, content);
            }
        }
    }
}