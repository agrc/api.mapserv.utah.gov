using System;
using System.Collections.Generic;
using System.Linq;
using api.mapserv.utah.gov.Models;
using EsriJson.Net;
using Point = EsriJson.Net.Geometry.Point;

namespace api.mapserv.utah.gov
{
    public class ConvertToEsriGraphicCommand : Command<ApiResponseContainer<Graphic>>
    {
        public ApiResponseContainer<GeocodeAddressApiResponse> Container { get; }

        public ConvertToEsriGraphicCommand(ApiResponseContainer<GeocodeAddressApiResponse> container)
        {
            Container = container;
        }

        public override string ToString() => "ConvertToEsriGraphicCommand";

        protected override void Execute()
        {
            EsriJsonObject geometry = null;
            var message = Container.Message;
            var status = Container.Status;
            var result = Container.Result;

            var attributes = GetProperties(result);

            if (result.Location != null)
            {
                geometry = new Point(result.Location.X, result.Location.Y)
                {
                    CRS = new Crs
                    {
                        WellKnownId = result.Wkid
                    }
                };
            }

            var graphic = new Graphic(geometry, attributes);

            var responseContainer = new ApiResponseContainer<Graphic>
            {
                Result = graphic,
                Status = status,
                Message = message
            };

            Result = responseContainer;
        }

        public static Dictionary<string, object> GetProperties<T>(T obj)
        {
            var properties = typeof(T).GetProperties();

            var dictionary = properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));

            return dictionary.Where(x => x.Value != null)
                             .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
