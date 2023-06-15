using System.Text.Json.Serialization;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using Asp.Versioning;
using EsriJson.Net;

namespace AGRC.api.Features.Converting;
public class EsriGraphic {
    public class Computation(SingleGeocodeResponseContract container, ApiVersion? version) : IComputation<SerializableGraphic> {
        public readonly SingleGeocodeResponseContract _response = container;
        public readonly int _version = version?.MajorVersion switch {
            2 => 2,
            _ => 1
        };
    }

    public class Handler(ILogger log) : IComputationHandler<Computation, SerializableGraphic> {
        private readonly ILogger? _log = log?.ForContext<EsriGraphic>();

        public Task<SerializableGraphic> Handle(Computation request, CancellationToken cancellationToken) {
            EsriJsonObject? geometry = null;
            var attributes = new Dictionary<string, object?>();
            var result = request._response;

            _log?.Debug("converting {result} to esri json for version {version}", result, request._version);

            if (result?.Location != null) {
                geometry = new EsriJson.Net.Geometry.Point(result.Location.X, result.Location.Y) {
                    CRS = new Crs {
                        WellKnownId = result.Wkid
                    }
                };

                var properties = request._response?
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

                if (request._version == 1) {
                    attributes = properties.ToDictionary(key => key.Name, value => value.GetValue(request._response, null))
                    ?? attributes;
                } else {
                    attributes = properties
                        .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                        .ToDictionary(key => key.Name, value => value.GetValue(request._response, null))
                        ?? attributes;

                    attributes.Remove("Location");

                    if (attributes.Values.Any(x => x == null)) {
                        attributes = attributes.Where(x => x.Value != null)
                            .ToDictionary(x => x.Key, y => y.Value);
                    }
                }
            }

            if (geometry == null && attributes.Count < 1) {
                return Task.FromResult(new SerializableGraphic(new Graphic(null, null)));
            }

            var graphic =
                new Graphic(geometry, attributes
                    .ToDictionary(x => x.Key, y => y.Value));

            var responseContainer = new SerializableGraphic(graphic);

            return Task.FromResult(responseContainer);
        }
    }

    public class SerializableGraphic(Graphic graphic) {
        public Dictionary<string, object> Attributes { get; } = graphic.Attributes;
        public object Geometry { get; set; } = graphic.Geometry;
    }
}
