using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SOESupport;
using Soe.Common.Attributes;
using Soe.Common.Extensions;
using Soe.Common.GDB.Connect;
using Soe.Common.Infastructure.Commands;
using Soe.Common.Infastructure.Endpoints;
using Soe.Common.Models.Esri;
using WebAPI.Domain.Comparers;
using WebAPI.Domain.DataStructures;
using WebAPI.Domain.ReverseMilepostModels;
using WebAPI.Search.Soe.Commands;
using WebAPI.Search.Soe.Extensions;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Endpoints
{
    [Endpoint]
    public class ReverseMilepost : JsonEndpoint, IRestEndpoint
    {
        /// <summary>
        ///     The resource name that displays for Supported Operations
        /// </summary>
        private const string ResourceName = "ReverseMilepost";

        /// <summary>
        ///     A method that the dynamic endpoint setup uses for registering the rest endpoing operation details.
        /// </summary>
        /// <returns> </returns>
        public RestOperation RestOperation()
        {
            return new RestOperation(ResourceName,
                                     new[]
                                         {
                                             "x", "y", "wkid", "buffer"
                                         },
                                     new[]
                                         {
                                             "json"
                                         }, Handler);
        }


        private static double GetDistanceBetween(IPoint from, IPoint to)
        {
            var dx = from.X - to.X;
            var dy = from.Y - to.Y;

            var d = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            var distance = Math.Sqrt(d);

            return distance;
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
            const string featureClass = "SGID10.TRANSPORTATION.UDOTRoutes_LRS";

            //pull out all the variables
            var x = operationInput.GetNumberValue("x");
            var y = operationInput.GetNumberValue("y");
            var wkidInput = operationInput.GetNumberValue("wkid", nullable: true);
            var bufferInput = operationInput.GetNumberValue("buffer", nullable: true);
            var includeRamps = operationInput.GetNumberValue("includeRamps", nullable: true);

            ISpatialReference newSpatialRefefence = null;
            ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironment();

            if (wkidInput > 0)
            {
                wkid = Convert.ToInt32(wkidInput);
            }

            if (bufferInput < 1 || bufferInput > 200)
            {
                bufferInput = 100;
            }

            //reproject to our data's spatial reference
            if (wkid != 26912)
            {
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

            var utm = srFactory.CreateProjectedCoordinateSystem(26912);

            IPoint point = new Point
                {
                    X = x,
                    Y = y,
                    SpatialReference = utm
                };

            //input is in different projection - reproject it
            if (wkid != 26912)
            {
                point = new Point
                    {
                        X = x,
                        Y = y,
                        SpatialReference = newSpatialRefefence
                    };

                point.Project(utm);
            }

            var bufferGeometry = CommandExecutor.ExecuteCommand(
                new BufferGeometryCommand(new GeometryContainer
                    {
                        Geometry = point
                    }, bufferInput));

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

            var featureWorkspace = workspace as IFeatureWorkspace;

            if (featureWorkspace == null)
            {
                errors.Add("Error connecting to SDE.");
                return Json(errors);
            }

            var whereClause = "(LABEL LIKE '0%') AND RT_DIR <> 'B'"; //gets rid of ramps, collectors, and federal aid routes
            if (includeRamps < 1)
            {
                whereClause = whereClause.Insert(0, "LEN(LABEL) = 5 AND ");
            }

            var spatFilter = new SpatialFilterClass
                {
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    Geometry = bufferGeometry,
                    WhereClause = whereClause
                };

            var lrsFeatureClass = featureWorkspace.OpenFeatureClass(featureClass.Trim());
            var fCursor = lrsFeatureClass.Search(spatFilter, true);
            var candidates = new TopAndEqualMilepostCandidates(new ClosestMilepostComparer());

            IFeature row;
            while ((row = fCursor.NextFeature()) != null)
            {
                var shape = row.ShapeCopy as IPolyline;
                if (shape == null)
                {
                    continue;
                }

                IPoint hitPoint = new Point();
                var hitDistance = -1d;
                var hitPartIndex = -1;
                var hitSegmentIndex = -1;
                var increasingSide = false;

                var hitTest = shape as IHitTest;
                if (hitTest == null)
                {
                    continue;
                }

                var isHit = hitTest.HitTest(point, bufferInput, esriGeometryHitPartType.esriGeometryPartBoundary, hitPoint,
                                ref hitDistance, ref hitPartIndex, ref hitSegmentIndex, ref increasingSide);

                if (!isHit)
                {
                    continue;
                }

                var milepost = hitPoint.M;

                var distance = GetDistanceBetween(point, hitPoint);

                var routeName = CommandExecutor.ExecuteCommand(new GetValueForFieldCommand("LABEL", row.Fields, row));

                candidates.Add(new ClosestMilepost(milepost, routeName.ToString(), distance, increasingSide));

                Marshal.ReleaseComObject(row);
                Marshal.ReleaseComObject(shape);
                Marshal.ReleaseComObject(hitPoint);
            }

            Marshal.ReleaseComObject(lrsFeatureClass);
            Marshal.ReleaseComObject(workspace);
            Marshal.ReleaseComObject(spatFilter);

            return Json(candidates);
        }
    }
}