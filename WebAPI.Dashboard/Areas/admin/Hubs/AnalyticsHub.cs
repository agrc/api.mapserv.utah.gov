using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using WebAPI.Dashboard.Areas.admin.Services;

namespace WebAPI.Dashboard.Areas.admin.Hubs
{
    public class AnalyticsHub : Hub
    {
        private readonly RealTimeRequestCountEmitter _realTimeRequestCountEmitter;

        public AnalyticsHub() : this(RealTimeRequestCountEmitter.Instance) { }

        public AnalyticsHub(RealTimeRequestCountEmitter realTimeRequestCountEmitter)
        {
            _realTimeRequestCountEmitter = realTimeRequestCountEmitter;
        }

        public override Task OnConnected()
        {
            App.ConnectedClients.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            App.ConnectedClients.Remove(Context.ConnectionId);
            return base.OnDisconnected();
        }

        public dynamic[] GetLiveRequests()
        {
            return _realTimeRequestCountEmitter.GetLiveRequests();
        }
    }
}