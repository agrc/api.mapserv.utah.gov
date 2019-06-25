using System;
using System.Collections.Specialized;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Attributes;
using Soe.Common.Extensions;
using Soe.Common.GDB.Connect;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.Endpoints;
using Wsut.Upgrade.Soe.Commands;

namespace Wsut.Upgrade.Soe.Endpoints
{
    [Endpoint]
    public class RouteMilePostRestEndpoint : JsonEndpoint, IRestEndpoint
    {
        private const string LayerName = "SGID10.TRANSPORTATION.UDOTROUTES_LRS";

        #region IRestEndpoint Members

        public RestOperation RestOperation()
        {
            return new RestOperation("RouteMilePost",
                                     new[]
                                         {
                                             "milepost", "routeNumber", "routeType"
                                         },
                                     new[]
                                         {
                                             "json"
                                         },
                                     Handler);
        }

        #endregion

        public static byte[] Handler(NameValueCollection boundVariables, JsonObject operationInput,
                                     string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            double? milepost = null;
            double? routeNumber = null;
            string routeType = null;
            string route = null;
            bool? fullRoute;

            var found = operationInput.TryGetAsDouble("milepost", out milepost);
            if (!found || !milepost.HasValue)
            {
                throw new ArgumentNullException("milepost");
            }

            found = operationInput.TryGetAsBoolean("fullRoute", out fullRoute);

            if (found && fullRoute.HasValue && fullRoute.Value)
            {
                found = operationInput.TryGetString("routeNumber", out route);
                if (!found || string.IsNullOrEmpty(route))
                {
                    throw new ArgumentNullException("route");
                }
            }
            else
            {
                found = operationInput.TryGetAsDouble("routeNumber", out routeNumber);
                if (!found || !routeNumber.HasValue)
                {
                    throw new ArgumentNullException("routeNumber");
                }

                found = operationInput.TryGetString("routeType", out routeType);
                if (!found || string.IsNullOrEmpty(routeType))
                {
                    throw new ArgumentNullException("routeType");
                }
            }

            var connector = SdeConnectorFactory.Create(LayerName);

            if (connector == null)
            {
                return Json(new
                    {
                        Message = "Database does not exist for {0}".With(LayerName)
                    });
            }

            var workspace = connector.Connect();

            var featureWorkSpace = workspace as IFeatureWorkspace;

            if (featureWorkSpace == null)
            {
                return Json(new
                    {
                        Message = "Error connecting to SDE."
                    });
            }

            FindRouteMilepostCommand command = null;

            if (fullRoute.HasValue && fullRoute.Value)
            {
                command = new FindRouteMilepostCommand(route, milepost.Value.ToString(CultureInfo.InvariantCulture), LayerName,
                                                       featureWorkSpace);
            }
            else
            {
                command = new FindRouteMilepostCommand(milepost.Value.ToString(CultureInfo.InvariantCulture),
                                                                            routeType,
                                                                            routeNumber.Value.ToString("0000"),
                                                                            LayerName,
                                                                            featureWorkSpace);
            }

            var response = CommandExecutor.ExecuteCommand(command);

            if (response == null)
            {
                var message = routeNumber + routeType;
                if (string.IsNullOrEmpty(message))
                {
                    message = route;
                }

                return Json(new
                    {
                        Message = "No mile post found for {0} on route {1}".With(milepost, message)
                    });
            }

            return Json(response);
        }
    }
}
