using System.Text.Json.Serialization;
using ugrc.api.Models.Linkables;

namespace ugrc.api.Features.Converting;

/// <summary>
/// Custom JSON converter for GridLinkable abstract class.
/// Handles polymorphic serialization/deserialization by detecting concrete types based on their unique properties.
/// </summary>
public class GridLinkableJsonConverter : JsonConverter<GridLinkable> {
    public override GridLinkable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Detect concrete type based on unique properties
        // UspsDeliveryPointLink has MatchAddress property and extends ZipGridLink
        if (root.TryGetProperty("MatchAddress", out _)) {
            return JsonSerializer.Deserialize<UspsDeliveryPointLink>(root.GetRawText(), options);
        }

        // ZipGridLink has ZipCode property
        if (root.TryGetProperty("ZipCode", out _)) {
            return JsonSerializer.Deserialize<ZipGridLink>(root.GetRawText(), options);
        }

        // PlaceGridLink has City property
        if (root.TryGetProperty("City", out _)) {
            return JsonSerializer.Deserialize<PlaceGridLink>(root.GetRawText(), options);
        }

        throw new JsonException($"Unable to determine GridLinkable concrete type from JSON");
    }

    public override void Write(Utf8JsonWriter writer, GridLinkable value, JsonSerializerOptions options) =>
        // Serialize as the concrete runtime type to preserve all properties
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
}
