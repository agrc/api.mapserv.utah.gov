using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EsriJson.Net.Geometry;

namespace EsriJson.Net.Geometry.Converters {
    public class RingPointConverter : JsonConverter<RingPoint> {
        public override void Write(Utf8JsonWriter writer, RingPoint value, JsonSerializerOptions options) {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteEndArray();
        }

        public override RingPoint Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => null;
    }
}
