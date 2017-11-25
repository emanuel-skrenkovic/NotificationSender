using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface IHttpService
    {
        void SendGetRequest(string url);

        Task SendGetRequestAsync(string url);

        void SendPostRequest<T>(string url, T body);

        Task SendPostRequestAsync<T>(string url, T body);
    }
}
