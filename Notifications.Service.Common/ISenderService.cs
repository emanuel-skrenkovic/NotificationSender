using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface ISenderService
    {
        TResponse Send<TResponse>(string message, string host, int port) where TResponse : class;

        TResponse Send<TMessage, TResponse>(TMessage messageObj, string host, int port)
            where TMessage : class
            where TResponse : class;

        Task<TResponse> SendAsync<TResponse>(string message, string host, int port) where TResponse : class;

        Task<TResponse> SendAsync<TMessage, TResponse>(TMessage messageObj, string host, int port)
            where TMessage : class
            where TResponse : class;

        List<TResponse> SendBatch<TResponse>(string message, List<string> hosts, int port) where TResponse : class;

        List<TResponse> SendBatch<TMessage, TResponse>(TMessage messageObj, List<string> hosts, int port)
            where TMessage : class
            where TResponse : class;

        Task<List<TResponse>> SendBatchAsync<TResponse>(string message, List<string> hosts, int port) where TResponse : class;

        Task<List<TResponse>> SendBatchAsync<TMessage, TResponse>(TMessage messageObj, List<string> hosts, int port)
            where TMessage : class
            where TResponse : class;
    }
}
