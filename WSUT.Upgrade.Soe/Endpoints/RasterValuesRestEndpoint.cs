#region License

// 
// Copyright (C) 2012 AGRC
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using System;
using System.Collections.Specialized;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Attributes;
using Soe.Common.Extensions;
using Soe.Common.GDB.Connect;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.Endpoints;
using Soe.Common.Models.Esri;
using WSUT.Upgrade.Soe.Models.Args;
using Wsut.Upgrade.Soe.Commands;

namespace Wsut.Upgrade.Soe.Endpoints
{
    /// <summary>
    ///     A rest endpoint. All endpoints are marked with the [Endpoint] attribute and dynamically added to the implmentation
    ///     at registration time
    /// </summary>
    [Endpoint]
    public class RasterValuesRestEndpoint : JsonEndpoint, IRestEndpoint
    {
        /// <summary>
        ///     The resource name that displays for Supported Operations
        /// </summary>
        private const string ResourceName = "GetRasterValue";

        #region IRestEndpoint Members

        /// <summary>
        ///     A method that the dynamic endpoint setup uses for registering the rest endpoing operation details.
        /// </summary>
        /// <returns> </returns>
        public RestOperation RestOperation()
        {
            return new RestOperation(ResourceName,
                                     new[]
                                         {
                                             "layerName", "utmx", "utmy"
                                         },
                                     new[]
                                         {
                                             "json"
                                         },
                                     Handler);
        }

        #endregion

        /// <summary>
        ///     Handles the incoming rest requests
        /// </summary>
        /// <param name="boundVariables"> The bound variables. </param>
        /// <param name="operationInput"> The operation input. </param>
        /// <param name="outputFormat"> The output format. </param>
        /// <param name="requestProperties"> The request properties. </param>
        /// <param name="responseProperties"> The response properties. </param>
        /// <returns> </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static byte[] Handler(NameValueCollection boundVariables, JsonObject operationInput,
                                     string outputFormat, string requestProperties,
                                     out string responseProperties)
        {
            responseProperties = null;
            var errors = new ErrorModel(400);

            string layerName;
            double? utmx, utmy;

            var found = operationInput.TryGetString("layerName", out layerName);
            if (!found || string.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException("layerName");
            }

            found = operationInput.TryGetAsDouble("utmx", out utmx);
            if (!found || !utmx.HasValue)
            {
                throw new ArgumentNullException("utmx");
            }

            found = operationInput.TryGetAsDouble("utmy", out utmy);
            if (!found || !utmy.HasValue)
            {
                throw new ArgumentNullException("utmy");
            }

            var connector = SdeConnectorFactory.Create(layerName);

            if (connector == null)
            {
                return Json(new
                    {
                        Message = "Database does not exist for {0}".With(layerName)
                    });
            }

            var workspace = connector.Connect();

            var featureWorkSpace = workspace as IFeatureWorkspace;

            if (featureWorkSpace == null)
            {
                errors.Message = "Error connecting to SDE.";

                return Json(errors);
            }

            var rasterArgs = new RasterArgs(layerName, utmx, utmy);

            var response =
                CommandExecutor.ExecuteCommand(new GetValueFromRasterCommand(rasterArgs,
                                                                             featureWorkSpace as IRasterWorkspaceEx));

            if (response == null)
            {
                errors.Message = "No features found in {2} at the location {0}, {1}.".With(
                    rasterArgs.X, rasterArgs.Y, rasterArgs.LayerName);

                return Json(errors);
            }

            return Json(response);
        }
    }
}