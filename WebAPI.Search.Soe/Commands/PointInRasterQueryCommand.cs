using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Soe.Common.Extensions;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;
using WebAPI.Search.Soe.Models.Results;

namespace WebAPI.Search.Soe.Commands
{
    public class PointInRasterQueryCommand : Command<ResponseContainer<SearchResult>>
    {
        private readonly ResponseContainer<SearchResult> _container = new ResponseContainer<SearchResult>();

        public PointInRasterQueryCommand(IRasterWorkspaceEx featureWorkspace, SpatialQueryArgs args)
        {
            FeatureWorkspace = featureWorkspace;
            Args = args;

            _container.Results = new List<SearchResult>();
        }

        public IRasterWorkspaceEx FeatureWorkspace { get; set; }
        public SpatialQueryArgs Args { get; set; }

        /// <summary>
        /// code to execute when command is run.
        /// </summary>
        protected override void Execute()
        {
            IList<string> errors = new List<string>();

            if (Args.Shape == null)
            {
                ErrorMessage = "To query a raster you must supply a shape";
                return;
            }

            IGeometry point = null;

            if (Args.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                point = Args.Shape;
            }
            else
            {
                var area = Args.Shape as IArea;
                if (area == null)
                {
                    errors.Add("Could not get centroid from input geometry. Best results are with point geometry types.");
                }
                else
                {
                    point = area.Centroid;
                }
            }

            if (errors.Any())
            {
                ErrorMessage = "Could not complete operation. {0}".With(string.Join(", ", errors));
                return;
            }

            var featureClass = FeatureWorkspace.OpenRasterDataset(Args.FeatureClass.Trim());

            var rasterLayer = new RasterLayer();
            rasterLayer.CreateFromDataset(featureClass);

            var table = rasterLayer as ITable;

            if (table == null)
            {
                ErrorMessage = "Could not complete operation. {0}".With("Accessing raster table.");
                return; 
            }

            errors = ValidateFields(table, Args.ReturnValues);

            if (errors.Any())
            {
                ErrorMessage = "{0} does not contain an attribute {1}. Check your spelling."
                    .With(Args.FeatureClass, string.Join(" or ", errors));

                return;
            }

            var identify = rasterLayer as IIdentify;

            if (identify == null)
            {
                ErrorMessage = "Could not complete operation. {0}".With("Identifying raster.");
                return;
            }

            var results = identify.Identify(point);
            if (results == null || results.Count < 1)
            {
                Result = new ResponseContainer<SearchResult>
                {
                    Results = new List<SearchResult>()
                };

                return;
            }

            var container = AddResultsToContainer(results, table.Fields, Args.ReturnValues, Args.SpatialRefefence);

            Marshal.ReleaseComObject(identify);
            Marshal.ReleaseComObject(table);
            Marshal.ReleaseComObject(rasterLayer);
            Marshal.ReleaseComObject(featureClass);
            Marshal.ReleaseComObject(FeatureWorkspace);

            Result = container;
        }

        protected virtual ResponseContainer<SearchResult> AddResultsToContainer(IArray items, IFields fields, string[] returnValues, ISpatialReference spatialReference = null)
        {
            var container = new ResponseContainer<SearchResult>
            {
                Results = new List<SearchResult>()
            };

            for (var i = 0; i < items.Count; i++)
            {
                var item = items.Element[i] as IRasterIdentifyObj2;

                var feature = new SearchResult
                    {
                        Attributes = new Dictionary<string, object>()
                    };

                foreach (var attribute in returnValues)
                {
                    var cleantribute = attribute.Trim().ToUpperInvariant();
                    if (cleantribute.Contains("SHAPE@"))
                    {   
                        continue;
                    }

                    var result = GetValueForField(cleantribute, fields, item);
                    if (result == null)
                    {
                        continue;
                    }

                    feature.Attributes.Add(attribute.Trim(), result);
                }

                if (!feature.Attributes.Any())
                {
                    continue;
                }

                container.Results.Add(feature);
            }

            return container;
        }

        protected virtual IList<string> ValidateFields(ITable featureClass, IEnumerable<string> returnValues)
        {
            var errors = new Collection<string>();

            foreach (var field in returnValues)
            {
                if (!field.ToUpperInvariant().StartsWith("SHAPE@"))
                {
                    if (featureClass.FindField(field) < 0)
                    {
                        errors.Add("'{0}'".With(field));
                    }
                }
            }

            return errors;
        }

        private static object GetValueForField(string attributeName, IFields fields, IRasterIdentifyObj2 row)
        {
            var findField = fields.FindField(attributeName.Trim());

            if (findField < 0)
            {
                throw new ArgumentException(attributeName + " "); 
            }

            string value, property;

            try
            {
                row.GetPropAndValues(findField, out property, out value);
            }
            catch (COMException)
            {
                // NoData pixel found
                value = null;
            }

            return value;
        }

        public override string ToString()
        {
            return string.Format("{0}, Args: {1}", "PointInPolygonQueryCommand", Args);
        }
    }
}