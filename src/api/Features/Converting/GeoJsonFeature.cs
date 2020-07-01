using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json.Linq;
using Point = GeoJSON.Net.Geometry.Point;

namespace api.mapserv.utah.gov.Features.Converting {
    public class GeoJsonFeature {
        public class Computation : IComputation<ApiResponseContainer<Feature>> {
            internal readonly ApiResponseContainer<GeocodeAddressApiResponse> Container;

            public Computation(ApiResponseContainer<GeocodeAddressApiResponse> container) {
                Container = container;
            }
        }

        public class Handler : IComputationHandler<Computation, ApiResponseContainer<Feature>> {
            public Task<ApiResponseContainer<Feature>> Handle(Computation request, CancellationToken cancellationToken) {
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
                    return Task.FromResult(new ApiResponseContainer<Feature> {
                        Status = status,
                        Message = message
                    });
                }

                var feature =
                    new Feature(geometry,
                                attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContainer<Feature> {
                    Result = feature,
                    Status = status,
                    Message = message
                };

                return Task.FromResult(responseContainer);
            }
        }
    }
}
