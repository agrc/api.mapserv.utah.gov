using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using MediatR;
using Newtonsoft.Json.Linq;
using Point = GeoJSON.Net.Geometry.Point;

namespace api.mapserv.utah.gov.Features.Converting {
    public class GeoJsonFeature {
        public class Command : IRequest<ApiResponseContainer<Feature>> {
            internal readonly ApiResponseContainer<GeocodeAddressApiResponse> Container;

            public Command(ApiResponseContainer<GeocodeAddressApiResponse> container) {
                Container = container;
            }
        }

        public class Handler : RequestHandler<Command, ApiResponseContainer<Feature>> {
            protected override ApiResponseContainer<Feature> Handle(Command request) {
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
                    return new ApiResponseContainer<Feature> {
                        Status = status,
                        Message = message
                    };
                }

                var feature =
                    new Feature(geometry,
                                attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContainer<Feature> {
                    Result = feature,
                    Status = status,
                    Message = message
                };

                return responseContainer;
            }
        }
    }
}
