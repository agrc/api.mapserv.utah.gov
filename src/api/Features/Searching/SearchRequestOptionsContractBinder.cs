using System;
using System.Threading.Tasks;
using AGRC.api.Models.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;

namespace AGRC.api.Features.Searching {
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
                var routeValues = bindingContext.ActionContext.RouteData.Values;

                routeValues.TryGetValue(versionKey, out var versionValue);
                var version = Convert.ToInt16(versionValue);

                if (version == 2) {
                    attributeStyle = AttributeStyle.Input;
                }
            } else {
                Enum.TryParse(attribute, out AttributeStyle attributeStyle);
            }

            var result = new SearchRequestOptionsContract {
                Predicate = predicate,
                Geometry = pointJson,
                Buffer = Convert.ToDouble(bufferAmount),
                AttributeStyle = attributeStyle
            };

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
