using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface INetworkService
    {

        string GetIpString();

        IPAddress GetIp();

        List<string> GetClients();

        Task<List<string>> GetClientsAsync();

        IPAddress GetDefaultGateway();

        string GetSubnet();
    }
}
