using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ResponseObjects;
using EsriJson.Net;
using EsriJson.Net.Geometry;
using MediatR;
using Newtonsoft.Json.Linq;

namespace api.mapserv.utah.gov.Features.Converting {
    public class EsriGraphic {
        public class Command : IRequest<ApiResponseContainer<Graphic>> {
            internal readonly ApiResponseContainer<GeocodeAddressApiResponse> Container;

            public Command(ApiResponseContainer<GeocodeAddressApiResponse> container) {
                Container = container;
            }
        }

        public class Handler : RequestHandler<Command, ApiResponseContainer<Graphic>> {
            protected override ApiResponseContainer<Graphic> Handle(Command request) {
                EsriJsonObject geometry = null;
                var message = request.Container.Message;
                var status = request.Container.Status;
                var result = request.Container.Result;

                var attributes = JObject.FromObject(request.Container.Result)
                                        .ToObject<Dictionary<string, object>>();

                if (result.Location != null) {
                    geometry = new Point(result.Location.X, result.Location.Y) {
                        CRS = new Crs {
                            WellKnownId = result.Wkid
                        }
                    };
                }

                var graphic =
                    new Graphic(geometry,
                                attributes.Where(x => x.Value != null).ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContainer<Graphic> {
                    Result = graphic,
                    Status = status,
                    Message = message
                };

                return responseContainer;
            }
        }
    }
}
