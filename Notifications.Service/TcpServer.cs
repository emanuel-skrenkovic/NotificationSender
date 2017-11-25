using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class TcpServer : ITcpServer
    {
        private TcpListener listener;

        private Action<object, object> callback;
        private bool isRunning;

        public bool IsRunning { get { return isRunning; } }

        public TcpServer(string ipAddress, int port)
        {
            var address = IPAddress.Parse(ipAddress);
            listener = new TcpListener(address, port);
        }

        public void Start(Action<object, object> callback, object state)
        {
            this.callback = callback;

            isRunning = true;

            listener.Start();
            listener.BeginAcceptTcpClient(ProcessResult, state);
        }

        public void Stop()
        {
            listener.Stop();
            isRunning = false;
        }

        private void ProcessResult(IAsyncResult res)
        {
            InvokeCallback(res);

            listener.Start();
            listener.BeginAcceptTcpClient(ProcessResult, res.AsyncState);
        }

        private void InvokeCallback(IAsyncResult res)
        {
            using (var client = listener.EndAcceptTcpClient(res))
            using (var ns = client.GetStream())
            {
                var message = GetStringMessage(ns);

                try
                {
                    callback.Invoke(message, res.AsyncState);
                }
                catch (Exception e)
                {
                    throw new NotSupportedException();
                }

                client.Close();
            }
        }

        private string GetStringMessage(NetworkStream nStream)
        {
            return GetStringMessageAsync(nStream).Result;
        }

        private async Task<string> GetStringMessageAsync(NetworkStream nStream)
        {
            using (var streamReader = new StreamReader(nStream))
            {
                var stringMessage = await streamReader.ReadToEndAsync();

                return Regex.Replace(stringMessage, @"\t|\n|\r", "");
            }
        }
    }
}
