using System.Collections.Generic;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ResponseObjects;
using EsriJson.Net;
using Newtonsoft.Json.Linq;
using Point = EsriJson.Net.Geometry.Point;

namespace api.mapserv.utah.gov.Commands.Formatting
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

            var attributes = JObject.FromObject(Container.Result)
                                    .ToObject<Dictionary<string, object>>();

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
    }
}
