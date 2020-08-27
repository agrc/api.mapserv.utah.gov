using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json.Linq;
using AGRC.api.Models.ResponseContracts;

namespace AGRC.api.Features.Converting {
    public class GeoJsonFeature {
        public class Computation : IComputation<ApiResponseContract<Feature>> {
            internal readonly ApiResponseContract<SingleGeocodeResponseContract> Container;

            public Computation(ApiResponseContract<SingleGeocodeResponseContract> container) {
                Container = container;
            }
        }

        public class Handler : IComputationHandler<Computation, ApiResponseContract<Feature>> {
            public Task<ApiResponseContract<Feature>> Handle(Computation request, CancellationToken cancellationToken) {
                IGeometryObject geometry = null;
                var attributes = new Dictionary<string, object>();
                var message = request.Container.Message;
                var status = request.Container.Status;
                var result = request.Container.Result;

                if (result?.Location != null) {
                    geometry = new Point(new Position(result.Location.Y, result.Location.X));

                    attributes = JObject.FromObject(request.Container.Result)
                                        .ToObject<Dictionary<string, object>>();
                }

                if (geometry == null && attributes.Count < 1) {
                    return Task.FromResult(new ApiResponseContract<Feature> {
                        Status = status,
                        Message = message
                    });
                }

                var feature =
                    new Feature(geometry,
                                attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContract<Feature> {
                    Result = feature,
                    Status = status,
                    Message = message
                };

                return Task.FromResult(responseContainer);
            }
        }
    }
}