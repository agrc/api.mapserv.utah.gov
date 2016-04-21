using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class SearchOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            string pointJson = keyValueModel["geometry"],
                   spatialReference = keyValueModel["spatialReference"],
                   predicate = keyValueModel["predicate"],
                   attribute = keyValueModel["attributeStyle"],
                   bufferAmount = keyValueModel["buffer"];

            int wkid;
            var attributeType = AttributeStyle.Camel;

            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);

            try
            {
                attributeType =
                    (AttributeStyle)
                    Enum.Parse(typeof(AttributeStyle), string.IsNullOrEmpty(attribute) ? "Camel" : attribute, true);
            }
            catch (Exception)
            {
                /*ie sometimes sends display value in text box.*/
            }

            double buffer;
            double.TryParse(string.IsNullOrEmpty(bufferAmount) ? "0" : bufferAmount, out buffer);

            bindingContext.Model = new SearchOptions
                {
                    Geometry = pointJson,
                    Predicate = predicate,
                    WkId = wkid,
                    Buffer = buffer,
                    AttributeStyle = attributeType
                };

            return true;
        }
    }
}