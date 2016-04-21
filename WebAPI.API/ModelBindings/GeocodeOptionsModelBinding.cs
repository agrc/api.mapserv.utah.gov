using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class GeocodeOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        { 
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            string suggestCount = keyValueModel["suggest"],
                   acceptScore = keyValueModel["acceptScore"],
                   format = keyValueModel["format"],
                   locator = keyValueModel["locators"],
                   spatialReference = keyValueModel["spatialReference"],
                   pobox = keyValueModel["pobox"];

            var locatorType = LocatorType.All;
            var jsonFormat = JsonFormat.None;

            int count,
                score,
                wkid;

            bool geocodePoBox;

            int.TryParse(string.IsNullOrEmpty(acceptScore) ? "70" : acceptScore, out score);
            int.TryParse(string.IsNullOrEmpty(suggestCount) ? "0" : suggestCount, out count);
            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);
            bool.TryParse(string.IsNullOrEmpty(pobox) ? "false" : pobox, out geocodePoBox);

            try
            {
                jsonFormat =
                    (JsonFormat) Enum.Parse(typeof (JsonFormat), string.IsNullOrEmpty(format) ? "none" : format, true);
            } catch (Exception)
            {
                /*ie sometimes sends display value in text box.*/
            }

            try
            {
                locatorType =
                    (LocatorType)
                    Enum.Parse(typeof (LocatorType), string.IsNullOrEmpty(locator) ? "all" : locator, true);
            } catch (Exception)
            {
                /*ie sometimes sends display value in text box.*/
            }

            bindingContext.Model = new GeocodeOptions
            {
                SuggestCount = count,
                AcceptScore = score,
                JsonFormat = jsonFormat,
                Locators = locatorType,
                WkId = wkid,
                PoBox = geocodePoBox
            };

            return true;
        }
    }
}