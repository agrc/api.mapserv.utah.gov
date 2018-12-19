using System;
using System.Globalization;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace api.mapserv.utah.gov.Models.RequestOptions {
    public class SearchingOptionsBinder : IModelBinder {
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
                attributeStyle = (AttributeStyle)Enum.Parse(typeof(AttributeStyle), attribute, true);
            }

            var result = new SearchingOptions {
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
