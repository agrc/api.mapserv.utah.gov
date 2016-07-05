using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class ReverseGeocodeOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            var spatialReference = keyValueModel["spatialReference"];
            var distance = keyValueModel["distance"];

            int wkid;
            double distanceValue;

            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);
            double.TryParse(string.IsNullOrEmpty(distance) ? "5" : distance, out distanceValue);

            if (distanceValue < 5)
            {
                distanceValue = 5;
            }

            bindingContext.Model = new ReverseGeocodeOptions
            {
                Distance = distanceValue,
                WkId = wkid
            };

            return true;
        }
    }
}