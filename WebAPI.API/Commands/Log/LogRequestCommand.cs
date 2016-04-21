using System;
using Raven.Client;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Commands.Log
{
    public class LogRequestCommand : Command
    {
        private readonly IDocumentStore _store;
        public ApiKey Key { get; set; }
        public object Container { get; set; }
     
        public LogRequestCommand(ApiKey key, object container, IDocumentStore store)
        {
            _store = store;
            Key = key;
            Container = container;
        }

        public override string ToString()
        {
            return string.Format("{0}, Key: {1}", "LogRequestCommand", Key);
        }

        protected override void Execute()
        {
            using (var session = _store.OpenSession())
            {
                if (Container is ResultContainer<GeocodeAddressResult>)
                {
                    var container = Container as ResultContainer<GeocodeAddressResult>;
                    var result = container.Result;

                    var geocode =new GeocodeStreetZoneUsage(Key.Id,
                                               Key.AccountId,
                                               DateTime.UtcNow.Ticks,
                                               result.InputAddress,
                                               result.Score);

                    session.Store(geocode);
                }
                else if (Container is ResultContainer<RouteMilepostResult>)
                {
                    var container = Container as ResultContainer<RouteMilepostResult>;
                    var result = container.Result;

                    var route = new RouteMilepostUsage(Key.Id,
                                                       Key.AccountId,
                                                       DateTime.UtcNow.Ticks,
                                                       result.MatchRoute,
                                                       result.InputRouteMilePost);
                    session.Store(route);
                }
                else if (Container is ResultContainer<ReverseGeocodeResult>)
                {
                    var container = Container as ResultContainer<ReverseGeocodeResult>;
                    var result = container.Result;

                    var geocode = new ReverseGeocodeUsage(Key.Id,
                                               Key.AccountId,
                                               DateTime.UtcNow.Ticks,
                                               result.InputLocation.X,
                                               result.InputLocation.Y);

                    session.Store(geocode);

                }
                else if (Container is ResultContainer<MultipleGeocdeAddressResultContainer>)
                {
                    var container = Container as ResultContainer<MultipleGeocdeAddressResultContainer>;
                    var result = container.Result;

                    var i = 1;
                    var total = result.Addresses.Count;

                    foreach (var entry in result.Addresses)
                    {
                        session.Store(new MultipleGeocodeUsage(Key.Id,
                                                               Key.AccountId,
                                                               DateTime.UtcNow.Ticks,
                                                               string.IsNullOrEmpty(entry.InputAddress)? entry.ErrorMessage: entry.InputAddress,
                                                               entry.Score,
                                                               string.Format("{0} of {1}", i,total)));

                        i++;
                    }
                }
                else if (true)
                {
                    session.Store(new InfoFeatureClassNamesUsage(Key.Id, Key.AccountId,
                                                            DateTime.UtcNow.Ticks));
                }

                session.SaveChanges();
            }
        }
    }
}