using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using Raven.Client;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Areas.admin.Hubs;

namespace WebAPI.Dashboard.Areas.admin.Services
{
    public class RealTimeRequestCountEmitter
    {
        // Singleton instance
        private static readonly Lazy<RealTimeRequestCountEmitter> HubInstance =
            new Lazy<RealTimeRequestCountEmitter>(() =>
                                                  new RealTimeRequestCountEmitter(
                                                      GlobalHost.ConnectionManager.GetHubContext<AnalyticsHub>().Clients,
                                                      App.Kernel.Get<IDocumentStore>()));

        private readonly object _queryLock = new object();

        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(5000);
        private volatile bool _querying;


        private RealTimeRequestCountEmitter(IHubConnectionContext clients, IDocumentStore store)
        {
            Clients = clients;

            _timer = new Timer(QueryNewUsages, null, _updateInterval, _updateInterval);

            DocumentStore = store;
        }

        private IDocumentStore DocumentStore { get; set; }

        public static RealTimeRequestCountEmitter Instance
        {
            get { return HubInstance.Value; }
        }

        private IHubConnectionContext Clients { get; set; }

        public dynamic[] GetLiveRequests()
        {
            var grouped = new dynamic[] { new
                {
                    Date = new DateTime(DateTime.UtcNow.AddSeconds(-10).Ticks).ToLongTimeString(),
                    Requests = 0
                }, new
                {
                    Date = new DateTime(DateTime.UtcNow.AddSeconds(-5).Ticks).ToLongTimeString(),
                    Requests = 0
                }, new
                {
                    Date = new DateTime(DateTime.UtcNow.Ticks).ToLongTimeString(),
                    Requests = 0
                }};

            return grouped;
        }

        private void QueryNewUsages(object state)
        {
            lock (_queryLock)
            {
                if (!_querying)
                {
                    _querying = true;
                    if (App.ConnectedClients.Any())
                    {
                        using (var session = DocumentStore.OpenSession())
                        {
                            var fiveSecondsAgo = DateTime.UtcNow.AddMilliseconds(-5000).Ticks;

                            var requests = session.Query<GeocodeStreetZoneUsage>()
                                                  .Where(x => x.LastUsedTicks > fiveSecondsAgo)
                                                  .ToArray();

                            dynamic grouped;

                            if (!requests.Any())
                            {
                                grouped = new
                                    {
                                        Date = new DateTime(DateTime.UtcNow.Ticks).ToLongTimeString(),
                                        Requests = 0
                                    };
                            }
                            else
                            {
                                grouped = new
                                    {
                                        Date = new DateTime(requests.Last().LastUsedTicks).ToLongTimeString(),
                                        Requests = requests.Length,
                                    };
                            }

                            BroadcastApiUsage(new[] {grouped});
                        }
                    }

                    _querying = false;
                }
            }
        }

        private void BroadcastApiUsage(dynamic[] requests)
        {
            Clients.All.updateChart(requests);
        }
    }
}