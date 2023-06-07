namespace AGRC.api.Features.Searching;
public class SearchResponseContract {
    public object Geometry { get; set; } = default!;

    public IDictionary<string, object> Attributes { get; set; } = default!;
}
