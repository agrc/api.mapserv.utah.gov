using System.Text.Json.Serialization;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using EsriJson.Net;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Converting;
public class EsriGraphic {
    public class Computation : IComputation<ApiResponseContract<SerializableGraphic>> {
        internal readonly ApiResponseContract<SingleGeocodeResponseContract> Container;
        internal readonly int Version;

        public Computation(ApiResponseContract<SingleGeocodeResponseContract> container, ApiVersion? version) {
            Container = container;
            Version = version?.MajorVersion switch {
                1 => 1,
                2 => 2,
                _ => 1
            };
        }
    }

    public class Handler : IComputationHandler<Computation, ApiResponseContract<SerializableGraphic>> {
        private readonly ILogger? _log;
        public Handler(ILogger log) {
            _log = log?.ForContext<EsriGraphic>();
        }
        public Task<ApiResponseContract<SerializableGraphic>> Handle(Computation request, CancellationToken cancellationToken) {
            EsriJsonObject? geometry = null;
            var attributes = new Dictionary<string, object?>();
            var message = request.Container.Message;
            var status = request.Container.Status;
            var result = request.Container.Result;

            _log?.Debug("converting {result} to esri json for version {version}", result, request.Version);

            if (result?.Location != null) {
                geometry = new EsriJson.Net.Geometry.Point(result.Location.X, result.Location.Y) {
                    CRS = new Crs {
                        WellKnownId = result.Wkid
                    }
                };

                var properties = request.Container.Result?
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

                if (request.Version == 1) {
                    attributes = properties.ToDictionary(key => key.Name, value => value.GetValue(request.Container.Result, null))
                    ?? attributes;
                } else {
                    attributes = properties
                        .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                        .ToDictionary(key => key.Name, value => value.GetValue(request.Container.Result, null))
                        ?? attributes;

                    attributes.Remove("Location");

                    if (attributes.Values.Any(x => x == null)) {
                        attributes = attributes.Where(x => x.Value != null)
                            .ToDictionary(x => x.Key, y => y.Value);
                    }
                }
            }

            if (geometry == null && attributes.Count < 1) {
                return Task.FromResult(new ApiResponseContract<SerializableGraphic> {
                    Status = status,
                    Message = message
                });
            }

            var graphic =
                new Graphic(geometry, attributes
                    .ToDictionary(x => x.Key, y => y.Value));

            var responseContainer = new ApiResponseContract<SerializableGraphic> {
                Result = new SerializableGraphic(graphic),
                Status = status,
                Message = message
            };

            return Task.FromResult(responseContainer);
        }
    }

    public static class SerializableGraphicFactory {
        public static SerializableGraphic Create(Graphic graphic, ApiVersion version) =>
            version?.MajorVersion switch {
                1 => new SerializableGraphic(graphic),
                2 => new SerializableGraphic(graphic),
                _ => new SerializableGraphic(graphic)
            };
    }
    public class SerializableGraphic {
        public SerializableGraphic(Graphic graphic) {
            Attributes = graphic.Attributes;
            Geometry = graphic.Geometry;
        }

        public Dictionary<string, object> Attributes { get; }
        public object Geometry { get; set; }
    }
}
