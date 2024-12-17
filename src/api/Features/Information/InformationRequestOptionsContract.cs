using Microsoft.AspNetCore.WebUtilities;

namespace ugrc.api.Features.Information;
public class InformationRequestOptionsContract {
    /// <summary>
    /// The SGID category. All values will be returned if it is omitted.
    /// </summary>
    /// <example>
    /// boundaries
    /// </example>
    public string? SgidCategory {
        get;
        set => field = value?.ToLowerInvariant().Trim() ?? string.Empty;
    } = string.Empty;

    public static ValueTask<InformationRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

        keyValueModel.TryGetValue("sgidCategory", out var sgidCategory);
        keyValueModel.TryGetValue("category", out var category);

        var result = new InformationRequestOptionsContract {
            SgidCategory = !string.IsNullOrEmpty(sgidCategory) ? sgidCategory : category
        };

        return new ValueTask<InformationRequestOptionsContract>(result);
    }
}
