using System;
using System.Collections.Specialized;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Attributes;
using Soe.Common.Extensions;
using Soe.Common.GDB.Connect;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.Endpoints;
using Soe.Common.Models.Esri;
using WebAPI.Search.Soe.Commands;
using WebAPI.Search.Soe.Commands.Factory;
using WebAPI.Search.Soe.Extensions;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Endpoints
{
    [Endpoint]
    public class SearchEndpoint : JsonEndpoint, IRestEndpoint
    {
        /// <summary>
        ///     The resource name that displays for Supported Operations
        /// </summary>
        private const string ResourceName = "Search";

        /// <summary>
        ///     A method that the dynamic endpoint setup uses for registering the rest endpoing operation details.
        /// </summary>
        /// <returns> </returns>
        public RestOperation RestOperation()
        {
            return new RestOperation(ResourceName,
                                     new[]
                                         {
                                             "featureClass", "returnValues", "predicate", "geometry", "wkid", "buffer"
                                         },
                                     new[]
                                         {
                                             "json"
                                         }, Handler);
        }

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
            var errors = new ErrorContainer(400);
            var wkid = 26912;

            //pull out all the variables
            var featureClass = operationInput.GetStringValue("featureClass");
            var returnValues = operationInput.GetStringValue("returnValues");
            var predicate = operationInput.GetStringValue("predicate", nullable: true);
            var geometryJson = operationInput.GetStringValue("geometry", nullable: true);
            var wkidInput = operationInput.GetNumberValue("wkid", nullable: true);
            var bufferInput = operationInput.GetNumberValue("buffer", nullable: true);

            if (wkidInput > 0)
            {
                wkid = Convert.ToInt32(wkidInput);
            }

            var isSafeSqlCommand = new IsSafeSqlCommand(new[]
                {
                    featureClass, returnValues, predicate
                });

            if (!CommandExecutor.ExecuteCommand(isSafeSqlCommand))
            {
                errors.Add("Input appears to be unsafe. That is all I will tell you.");
            }

            GeometryContainer container = null;
            ISpatialReference newSpatialRefefence = null;
            ISpatialReferenceFactory srFactory = null;

            //reproject to our data's spatial reference
            if (wkid != 26912)
            {
                srFactory = new SpatialReferenceEnvironmentClass();

                var isProjected = true;
                try
                {
                    newSpatialRefefence = srFactory.CreateProjectedCoordinateSystem(wkid);
                }
                catch (ArgumentException)
                {
                    isProjected = false;
                }

                if (!isProjected)
                {
                    newSpatialRefefence = srFactory.CreateGeographicCoordinateSystem(wkid);
                }
            }

            //input has a geometry - deal with it
            if (!string.IsNullOrEmpty(geometryJson))
            {
                var extractGeometryCommand = new ExtractGeometryCommand(geometryJson, wkid);
                container = CommandExecutor.ExecuteCommand(extractGeometryCommand);

                if (container == null)
                {
                    var message = "Geometry coordinates appear to be invalid.";

                    if (!string.IsNullOrEmpty(extractGeometryCommand.ErrorMessage))
                    {
                        message += " Maybe this information can help: {0}".With(extractGeometryCommand.ErrorMessage);
                    }

                    errors.Add(message);
                }

                //input is in different projection - reproject it
                if (wkid != 26912)
                {
                    if (srFactory == null)
                    {
                        srFactory = new SpatialReferenceEnvironmentClass();
                    }

                    var toUtm = srFactory.CreateProjectedCoordinateSystem(26912);

                    if (container != null)
                    {
                        if ((container.Coordinates == null || container.Coordinates.Count < 1) && container.Geometry != null)
                        {
                            container.Geometry.Project(toUtm);
                        }
                        else
                        {
                            foreach (var points in container.Coordinates)
                            {
                                var point = new PointClass
                                {
                                    X = points[0],
                                    Y = points[1],
                                    SpatialReference = newSpatialRefefence
                                };

                                point.Project(toUtm);

                                if (point.IsEmpty)
                                {
                                    errors.Add("Input geometry is empty. Check your x and y values.");
                                    return Json(errors);
                                }

                                container.Geometry = point;

                                points[0] = point.X;
                                points[1] = point.Y;
                            }
                        }
                    }
                }

                //buffer point - set container type to polygon
                if (bufferInput > 0 && container != null)
                {
                    container.Geometry = CommandExecutor.ExecuteCommand(new BufferGeometryCommand(container, bufferInput));
                    container.Type = "POLYGON";
                }
            }

            var sdeConnector = SdeConnectorFactory.Create(featureClass);

            if (sdeConnector == null)
            {
                errors.Add("{0} was not found in our database. ".With(featureClass) +
                           "A valid example would be SGID10.BOUNDARIES.Counties.");
            }

            if (errors.HasErrors)
            {
                return Json(errors);
            }

// ReSharper disable PossibleNullReferenceException because of returning errors if null
            var workspace = sdeConnector.Connect();
            // ReSharper restore PossibleNullReferenceException


            if (workspace is not IFeatureWorkspace featureWorkspace)
            {
                errors.Add("Error connecting to SDE.");
                return Json(errors);
            }

            var values = returnValues.Split(',').Select(x => x.Trim()).ToArray();

            var commandToExecute = SpatialCommandFactory.Get(container, featureWorkspace, featureClass, values,
                                                             predicate, newSpatialRefefence);

            var result = CommandExecutor.ExecuteCommand(commandToExecute);

            if (!string.IsNullOrEmpty(commandToExecute.ErrorMessage))
            {
                errors.Add(commandToExecute.ErrorMessage);
                return Json(errors);
            }

            return Json(result);
        }
    }
}
