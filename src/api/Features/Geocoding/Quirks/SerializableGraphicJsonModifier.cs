using System.Text.Json.Serialization.Metadata;
using static AGRC.api.Features.Converting.EsriGraphic;

namespace AGRC.api.Quirks;
public class SerializableGraphicJsonModifier {
    public void ModifyAttributeConverter(JsonTypeInfo typeInfo) {
        if (typeInfo.Type != typeof(SerializableGraphic)) {
            return;
        }

        foreach (var propertyInfo in typeInfo.Properties) {
            if (!propertyInfo.Name.Equals("attributes", StringComparison.OrdinalIgnoreCase)) {
                continue;
            }

            // this property info is the attributes dictionary
            propertyInfo.CustomConverter = new EsriJsonAttributesConverter();
        }
    }
}
