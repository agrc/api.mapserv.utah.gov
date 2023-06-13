
namespace AGRC.api.Features.Searching;
public class SearchResponseContract {
    public object? Geometry { get; set; } = null;

    public IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
}
