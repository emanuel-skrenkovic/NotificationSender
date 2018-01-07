using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface IClient
    {
        TResponse Send<TResponse>(string message, string host, int port) where TResponse : class;

        Task<TResponse> SendAsync<TResponse>(string message, string host, int port) where TResponse : class;
    }
}
