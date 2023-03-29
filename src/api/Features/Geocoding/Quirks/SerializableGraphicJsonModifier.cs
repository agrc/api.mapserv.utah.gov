using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Logging;
using static AGRC.api.Features.Converting.EsriGraphic;
using ILogger = Microsoft.Extensions.Logging.ILogger;

#nullable enable
namespace AGRC.api.Quirks;
public class SerializableGraphicJsonModifier {
    private readonly ILogger _log;

    public SerializableGraphicJsonModifier(ILogger log) {
        _log = log;
    }
    public void ModifyAttributeConverter(JsonTypeInfo typeInfo) {
        if (typeInfo.Type != typeof(SerializableGraphic)) {
            return;
        }

        foreach (var propertyInfo in typeInfo.Properties) {
            if (!propertyInfo.Name.Equals("attributes", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            _log.LogInformation("property info {name}", propertyInfo.Name);

            // this property info is the attributes dictionary
            propertyInfo.CustomConverter = new EsriJsonAttributesConverter();
        }
    }
}
