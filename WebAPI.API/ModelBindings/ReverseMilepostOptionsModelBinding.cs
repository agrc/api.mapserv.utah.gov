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
                   includeRamps = keyValueModel["includeRampSystem"];

            var jsonFormat = JsonFormat.None;

            int wkid;
            double bufferDistance;
            bool includeRampSystems;

            double.TryParse(string.IsNullOrEmpty(buffer) ? "0" : buffer, out bufferDistance);
            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);
            bool.TryParse(string.IsNullOrEmpty(includeRamps) ? bool.FalseString : includeRamps, out includeRampSystems);

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
                    IncludeRampSystems = includeRampSystems ? 1 : 0
                };

            return true;
        }
    }
}