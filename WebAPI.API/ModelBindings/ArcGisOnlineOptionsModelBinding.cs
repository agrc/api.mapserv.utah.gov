using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class ArcGisOnlineOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            string address = keyValueModel["Single Line Input"],
                format = keyValueModel["f"],
                suggestCount = keyValueModel["maxLocations"],
                spatialReference = keyValueModel["outSR"];

            int count;

            int.TryParse(string.IsNullOrEmpty(suggestCount) ? "0" : suggestCount, out count);

            bindingContext.Model = new AgoGeocodeOptions
            {
                Address = address,
                SuggestCount = count,
                WkId = spatialReference,
                F = format
            };

            return true;
        }
    }
}