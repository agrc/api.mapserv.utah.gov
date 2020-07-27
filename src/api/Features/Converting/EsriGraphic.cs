using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using EsriJson.Net;
using Newtonsoft.Json.Linq;

namespace AGRC.api.Features.Converting {
    public class EsriGraphic {
        public class Computation : IComputation<ApiResponseContract<Graphic>> {
            internal readonly ApiResponseContract<SingleGeocodeResponseContract> Container;

            public Computation(ApiResponseContract<SingleGeocodeResponseContract> container) {
                Container = container;
            }
        }

        public class Handler : IComputationHandler<Computation, ApiResponseContract<Graphic>> {
            public Task<ApiResponseContract<Graphic>> Handle(Computation request, CancellationToken cancellationToken) {
                EsriJsonObject geometry = null;
                var attributes = new Dictionary<string, object>();
                var message = request.Container.Message;
                var status = request.Container.Status;
                var result = request.Container.Result;

                if (result?.Location != null) {
                    geometry = new EsriJson.Net.Geometry.Point(result.Location.X, result.Location.Y) {
                        CRS = new Crs {
                            WellKnownId = result.Wkid
                        }
                    };

                    attributes = JObject.FromObject(request.Container.Result)
                                        .ToObject<Dictionary<string, object>>();
                }

                if (geometry == null && attributes.Count < 1) {
                    return Task.FromResult(new ApiResponseContract<Graphic> {
                        Status = status,
                        Message = message
                    });
                }

                var graphic =
                    new Graphic(geometry,
                                attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContract<Graphic> {
                    Result = graphic,
                    Status = status,
                    Message = message
                };

                return Task.FromResult(responseContainer);
            }
        }
    }
}
