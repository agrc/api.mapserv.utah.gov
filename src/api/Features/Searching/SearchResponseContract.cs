using static AGRC.api.Features.Converting.EsriGraphic;

namespace AGRC.api.Features.Searching;
public class SearchResponseContract {
    public SerializableGraphic Geometry { get; set; } = default!;

    public IDictionary<string, object> Attributes { get; set; } = default!;
}
