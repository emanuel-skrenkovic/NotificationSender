using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface ITcpServer
    {
        bool IsRunning { get; }

        IAsyncResult Start(Action<object> callback, object state);

        void Stop();
    }
}
