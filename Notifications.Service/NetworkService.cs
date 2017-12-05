using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Diagnostics;

namespace Notifications.Service
{
    public class NetworkService : INetworkService
    {
        public List<string> GetAvailableNetworkPcs()
        {
            var replyList = new System.Collections.Concurrent.ConcurrentBag<PingReply>();

            var subnet = GetSubnet();

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

            Parallel.For(2, 256, (i) =>
            {
                string ip = $"{subnet}.{i}";

                using (var ping = new Ping())
                {
                    var reply = ping.Send(ip, 2);

                    if (reply.Status == IPStatus.Success)
                        replyList.Add(reply);
                }
            });

            var clientsList = new List<string>();

            foreach (var reply in replyList)
            {
                clientsList.Add(reply.Address.ToString());
            }

            return clientsList;
        }

        public async Task<List<string>> GetAvailableNetworkPcsAsync()
        {
            var pingList = new List<Task<PingReply>>();

            var subnet = GetSubnet();

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
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                var addr = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (addr != null)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public string GetIpString()
        {
            return GetIp().ToString();
        }

        public IPAddress GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                .FirstOrDefault();
        }

        public string GetSubnet()
        {
            var defaultGateway = GetDefaultGateway().ToString();
            return defaultGateway.Remove(defaultGateway.Length - 2);
        }
    }
}
