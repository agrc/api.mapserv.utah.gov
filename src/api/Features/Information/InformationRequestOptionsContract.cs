using Microsoft.AspNetCore.WebUtilities;

namespace ugrc.api.Features.Information;
public class InformationRequestOptionsContract {
    private string? _sgidCategory = string.Empty;

    /// <summary>
    /// The SGID category. All values will be returned if it is omitted.
    /// </summary>
    /// <example>
    /// boundaries
    /// </example>
    public string? SgidCategory {
        get => _sgidCategory;
        set => _sgidCategory = value?.ToLowerInvariant().Trim() ?? string.Empty;
    }

    public static ValueTask<InformationRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

        keyValueModel.TryGetValue("sgidCategory", out var sgidCategory);

        var result = new InformationRequestOptionsContract {
            SgidCategory = sgidCategory
        };

        return new ValueTask<InformationRequestOptionsContract>(result);
    }
}
