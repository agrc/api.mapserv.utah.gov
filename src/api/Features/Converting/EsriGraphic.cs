using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using EsriJson.Net;

namespace AGRC.api.Features.Converting {
    public class EsriGraphic {
        public class Computation : IComputation<ApiResponseContract<SerializableGraphic>> {
            internal readonly ApiResponseContract<SingleGeocodeResponseContract> Container;

            public Computation(ApiResponseContract<SingleGeocodeResponseContract> container) {
                Container = container;
            }
        }

        public class Handler : IComputationHandler<Computation, ApiResponseContract<SerializableGraphic>> {
            public Task<ApiResponseContract<SerializableGraphic>> Handle(Computation request, CancellationToken cancellationToken) {
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

                    attributes = request.Container.Result
                        .GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                        .ToDictionary(key => key.Name, value => value.GetValue(request.Container.Result, null));
                }

                if (geometry == null && attributes.Count < 1) {
                    return Task.FromResult(new ApiResponseContract<SerializableGraphic> {
                        Status = status,
                        Message = message
                    });
                }

                var graphic =
                    new Graphic(geometry, attributes
                        .Where(x => x.Value != null)
                        .ToDictionary(x => x.Key, y => y.Value));

                var responseContainer = new ApiResponseContract<SerializableGraphic> {
                    Result = new SerializableGraphic(graphic),
                    Status = status,
                    Message = message
                };

                return Task.FromResult(responseContainer);
            }
        }

        public class SerializableGraphic {
            public SerializableGraphic(Graphic graphic)
            {
                Attributes = graphic.Attributes;
                Geometry = graphic.Geometry;
            }

            public Dictionary<string, object> Attributes { get; private set; }
            public object Geometry { get; set; }
        }
    }
}
