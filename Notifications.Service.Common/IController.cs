using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service.Common
{
    public interface IController
    {
        object HandleRouting<TMessage>(string route, TMessage message);

        void RegisterRoute(string route, Delegate callback);
    }
}
