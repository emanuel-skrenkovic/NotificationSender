using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class TcpSender : ITcpSender, IDisposable
    {
        private TcpClient client;

        private bool disposedValue = false; // To detect redundant calls

        public TcpSender()
        {
            client = new TcpClient();
        }

        public void Send(string message, string host, int port)
        {
            client.Connect(host, port);

            WriteMessageToStream(message);

            client.Close();
        }

        public async Task SendAsync(string message, string host, int port)
        {
            await client.ConnectAsync(host, port);

            await WriteMessageToStreamAsync(message);

            client.Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                client.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        private void WriteMessageToStream(string message)
        {
            using (NetworkStream ns = client.GetStream())
            {
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

                ns.Write(bytesToSend, 0, bytesToSend.Length);
            }
        }

        private Task WriteMessageToStreamAsync(string message)
        {
            using (NetworkStream ns = client.GetStream())
            {
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

                return ns.WriteAsync(bytesToSend, 0, bytesToSend.Length);
            }
        }
    }
}
