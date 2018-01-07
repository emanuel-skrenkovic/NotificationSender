using Notifications.Common;
using Notifications.Models;
using Notifications.Service;
using Notifications.Service.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Client
{
    public class PingListener : BackgroundWorker
    {
        private IServer server;
        private ISenderService senderService;
        private INetworkService networkService;

        private UtilityService utility;

        public PingListener(IServer server, ISenderService senderService, INetworkService networkService, UtilityService utility)
        {
            this.server = server;
            this.senderService = senderService;
            this.networkService = networkService;

            this.utility = utility;
        }

        public void StartListen(object sender, DoWorkEventArgs e)   
        {
            if (server != null)
                server.Start(Routes.PingRoute, (Func<object, object, PingResponse>)HandlePing, null);
        }

        public void StopListen(object sender, DoWorkEventArgs e)
        {
            server.Stop();
        }

        private PingResponse HandlePing(object res, object state)
        {
            var pingReq = utility.DeserializeFromObject<PingRequest>(res);

            if (pingReq == null)
                return null;

            var pingResponse = new PingResponse
            {
                Address = networkService.GetIpString(),
            };

            return pingResponse;
        }
    }
}
