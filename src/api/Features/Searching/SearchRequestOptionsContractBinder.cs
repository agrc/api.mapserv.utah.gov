using AGRC.api.Models.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;

namespace AGRC.api.Features.Searching;
public class SearchRequestOptionsContractBinder : IModelBinder {
    private const string versionKey = "version";
    private AttributeStyle attributeStyle = AttributeStyle.Lower;

    public Task BindModelAsync(ModelBindingContext bindingContext) {
        if (bindingContext == null) {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var keyValueModel = QueryHelpers.ParseQuery(bindingContext.ActionContext.HttpContext.Request.QueryString.Value);

        keyValueModel.TryGetValue("geometry", out var pointJson);
        keyValueModel.TryGetValue("spatialReference", out var spatialReference);
        keyValueModel.TryGetValue("predicate", out var predicate);
        keyValueModel.TryGetValue("attributeStyle", out var attributeValue);
        keyValueModel.TryGetValue("buffer", out var bufferAmount);

        var attribute = attributeValue.ToString();

        if (string.IsNullOrEmpty(attribute)) {
            attributeStyle = AttributeStyle.Lower;

            var version = GetVersion(bindingContext);

            if (version == 2) {
                attributeStyle = AttributeStyle.Input;
            }
        } else {
            if (!Enum.TryParse(attribute, true, out attributeStyle)) {
                var version = GetVersion(bindingContext);
                if (version == 1) {
                    // reset to default as try parse modifies value
                    attributeStyle = AttributeStyle.Lower;
                } else if (version == 2) {
                    attributeStyle = AttributeStyle.Input;
                }
            }
        }

        var wkid = 26912;
        if (!string.IsNullOrEmpty(spatialReference)) {
            if (!int.TryParse(spatialReference, out wkid)) {
                // reset to default
                wkid = 26912;
            }
        }

        var result = new SearchRequestOptionsContract {
            Predicate = predicate,
            Geometry = pointJson,
            Buffer = Convert.ToDouble(bufferAmount),
            AttributeStyle = attributeStyle,
            SpatialReference = wkid
        };

        bindingContext.Result = ModelBindingResult.Success(result);

        return Task.CompletedTask;
    }

    private static int GetVersion(ModelBindingContext bindingContext) {
        var routeValues = bindingContext.ActionContext.RouteData.Values;

        routeValues.TryGetValue(versionKey, out var versionValue);

        return Convert.ToInt16(versionValue);
    }
}
