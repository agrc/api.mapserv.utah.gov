using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class ReverseMilepostOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        { 
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            string buffer = keyValueModel["buffer"],
                   format = keyValueModel["format"],
                   spatialReference = keyValueModel["spatialReference"],
                   includeRamps = keyValueModel["includeRampSystem"],
                   suggestCount = keyValueModel["suggest"];

            var jsonFormat = JsonFormat.None;

            int.TryParse(string.IsNullOrEmpty(suggestCount) ? "0" : suggestCount, out var count);
            double.TryParse(string.IsNullOrEmpty(buffer) ? "100" : buffer, out var bufferDistance);
            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out var wkid);
            bool.TryParse(string.IsNullOrEmpty(includeRamps) ? bool.FalseString : includeRamps, out var includeRampSystems);

            try
            {
                jsonFormat =
                    (JsonFormat) Enum.Parse(typeof (JsonFormat), string.IsNullOrEmpty(format) ? "none" : format, true);
            } catch (Exception)
            {
                /*ie sometimes sends display value in text box.*/
            }

            bindingContext.Model = new ReverseMilepostOptions
                {
                    Buffer = bufferDistance,
                    JsonFormat = jsonFormat,
                    WkId = wkid,
                    IncludeRampSystems = includeRampSystems ? 1 : 0,
                    SuggestCount = count
            };

            return true;
        }
    }
}