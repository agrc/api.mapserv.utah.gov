using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings.Providers
{
    public class ArcGisOnlineOptionsModelBindingProvide : ModelBinderProvider
    {
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            return modelType == typeof(AgoGeocodeOptions) ? new ArcGisOnlineOptionsModelBinding() : null;
        }
    }
}