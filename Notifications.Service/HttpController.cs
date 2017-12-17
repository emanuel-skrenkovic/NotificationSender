using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Service
{
    public class HttpController : IController
    {
        private Dictionary<string, Func<object, object, object>> registeredRoutes;

        private string baseRoute;

        public HttpController(string baseRoute)
        {
            this.baseRoute = baseRoute;
            registeredRoutes = new Dictionary<string, Func<object, object, object>>();
        }

        public object HandleRouting<TMessage>(string route, TMessage message)
        {
            var action = FindRoute(route);

            if (action == null)
                return null;

            return action.Invoke(message, null);
        }

        public void RegisterRoute(string routeName, Func<object, object, object> callback)
        {
            string fullRouteName = GetFullRouteName(routeName);
            
            if (!registeredRoutes.ContainsKey(fullRouteName))
            {
                registeredRoutes.Add(fullRouteName, callback);
            }
        }

        private Func<object, object, object> FindRoute(string route)
        {
            string fullRouteName = GetFullRouteName(route); 

            if (!registeredRoutes.ContainsKey(fullRouteName))
                return null;

            return registeredRoutes[fullRouteName];
        }
        
        private string GetFullRouteName(string route)
        {
            var sanitizedRoute = SanitizeRoute(route); 
            return $"{baseRoute}{sanitizedRoute}";
        }

        private string SanitizeRoute(string route)
        {
            if (route.EndsWith("/"))
                return route;
            else
                return $"{route}/";
        }
    }
}
