using System.Collections.Generic;
using EsriJson.Net.Geometry;

namespace EsriJson.Net;
public class Graphic(EsriJsonObject geometry, Dictionary<string, object> attributes) : IGeometry {
    public Dictionary<string, object> Attributes { get; } = attributes;

    public EsriJsonObject Geometry { get; set; } = geometry;
}
