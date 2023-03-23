using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace AGRC.api.Quirks;

public class EsriJsonAttributesConverter : JsonConverter<Dictionary<string, object>> {
    public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, Dictionary<string, object> dictionary, JsonSerializerOptions options) {
        writer.WriteStartObject();

        foreach ((var propertyName, var value) in dictionary) {
            if (propertyName.Equals("standardizedAddress", StringComparison.OrdinalIgnoreCase) && value is null) {
                continue;
            }

            writer.WritePropertyName
                (options.DictionaryKeyPolicy?.ConvertName(propertyName) ?? propertyName);

            if (propertyName.Equals("scoreDifference", StringComparison.OrdinalIgnoreCase) && value is null) {
                writer.WriteNumberValue(-1);
                continue;
            }

            if (propertyName.Equals("candidates", StringComparison.OrdinalIgnoreCase) && value is null) {
                writer.WriteStartArray();
                writer.WriteEndArray();

                continue;
            }

            JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
        }

        writer.WriteEndObject();
    }
}
