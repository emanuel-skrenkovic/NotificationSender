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

        Task<string> GetIpStringAsync();

        IPAddress GetIp();

        List<string> GetClients(string subnet);

        Task<List<string>> GetClientsAsync(string subnet);
    }
}
