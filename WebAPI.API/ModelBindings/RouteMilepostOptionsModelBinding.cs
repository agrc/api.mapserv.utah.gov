﻿using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using WebAPI.Domain;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.ModelBindings
{
    public class RouteMilepostOptionsModelBinding : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var keyValueModel = actionContext.Request.RequestUri.ParseQueryString();

            string side = keyValueModel["side"],
                   format = keyValueModel["format"],
                   spatialReference = keyValueModel["spatialReference"],
                   fullRoute = keyValueModel["fullRoute"];

            var jsonFormat = JsonFormat.None;

            bool.TryParse(string.IsNullOrEmpty(fullRoute) ? "false" : fullRoute, out var usingFullRouteName);

            int wkid;

            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);

            try
            {
                jsonFormat =
                    (JsonFormat) Enum.Parse(typeof (JsonFormat), string.IsNullOrEmpty(format) ? "none" : format, true);
            }
            catch (Exception)
            {
                /*ie sometimes sends display value in text box.*/
            }

            var sideType = SideDelineation.P;

            if (!string.IsNullOrEmpty(side) && side.ToUpperInvariant() == "DECREASING")
            {
                sideType = SideDelineation.N;
            }

            bindingContext.Model = new MilepostOptions
                {
                    Side = sideType,
                    JsonFormat = jsonFormat,
                    WkId = wkid,
                    FullRoute = usingFullRouteName
            };

            return true;
        }
    }
}