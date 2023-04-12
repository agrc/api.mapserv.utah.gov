using System.Collections.Generic;
using EsriJson.Net.Geometry;

namespace EsriJson.Net {
    public class Graphic : IGeometry {
        public Graphic(EsriJsonObject geometry, Dictionary<string, object> attributes) {
            Attributes = attributes;
            Geometry = geometry;
        }

        public Dictionary<string, object> Attributes { get; private set; }

        public EsriJsonObject Geometry { get; set; }
    }
}
