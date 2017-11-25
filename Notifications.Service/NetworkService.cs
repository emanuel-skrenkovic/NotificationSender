using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Notifications.Service
{
    public class NetworkService : INetworkService
    {
        public List<string> GetClients(string subnet)
        {
            var replyList = new List<PingReply>();

            for (int i = 2; i < 255; i++)
            {
                string ip = $"{subnet}.{i}";

                using (var ping = new Ping())
                {
                    var reply = ping.Send(ip, 2);

                    if (reply.Status == IPStatus.Success)
                        replyList.Add(reply);
                }
            }

            var clientsList = new List<string>();

            foreach (var reply in replyList)
            {
                clientsList.Add(reply.Address.ToString());
            }

            return clientsList;
        }

        public async Task<List<string>> GetClientsAsync(string subnet)
        {
            var pingList = new List<Task<PingReply>>();

            for (int i = 2; i < 255; i++)
            {
                string ip = $"{subnet}.{i}";

                using (var ping = new Ping())
                {
                    pingList.Add(ping.SendPingAsync(ip, 5));
                }
            }

            var replyList = await Task.WhenAll(pingList);

            var clientsList = new List<string>();

            foreach (var reply in replyList)
            {
                if (reply.Status == IPStatus.Success)
                {
                    clientsList.Add(reply.Address.ToString());
                }
            }

            return clientsList;
        }

        public IPAddress GetIp()
        {
            string hostName = Dns.GetHostName();

            return Dns.GetHostEntry(hostName).AddressList.First();
        }

        public string GetIpString()
        {
            string hostName = Dns.GetHostName();

            var test = Dns.GetHostEntry(hostName);

            return Dns.GetHostEntry(hostName).AddressList
                .Where(i => i.AddressFamily == AddressFamily.InterNetwork)
                .First()
                .ToString();
        }

        public async Task<string> GetIpStringAsync()
        {
            string hostName = Dns.GetHostName();

            var hostEntry = await Dns.GetHostEntryAsync(hostName);

            return hostEntry.AddressList
                .Where(i => i.AddressFamily == AddressFamily.InterNetwork)
                .First()
                .ToString();
        }
    }
}
