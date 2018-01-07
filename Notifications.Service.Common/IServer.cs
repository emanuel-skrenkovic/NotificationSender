using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface IServer
    {
        bool IsRunning { get; }

        void Start(string route, Delegate callback, object state);

        void Stop();
    }
}
