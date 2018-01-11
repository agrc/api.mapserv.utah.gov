using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
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

//            var rasterLayer = new RasterLayer();
//            rasterLayer.CreateFromDataset(featureClass);

            var table = ((IRasterBandCollection)featureClass).Item(0).AttributeTable;

            //            var rasterProps = featureClass.CreateDefaultRaster() as IRasterProps;
            //            var bufferDifference = (rasterProps.MeanCellSize().X / 2);
            //            var geometry = CommandExecutor.ExecuteCommand(new BufferGeometryCommand(new GeometryContainer { Geometry = point }, bufferDifference));

//            var table = rasterLayer as ITable; 

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

            var query = new SpatialFilterClass
            {
                Geometry = point,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIndexIntersects,
                SubFields = string.Join(",", Args.ReturnValues)
            };

            ICursor cursor;
            try
            {
                cursor = table.Search(query, false);
            }
            catch (Exception)
            {
                ErrorMessage = "Could not complete operation. {0}".With("Querying raster.");
                return;
            }

            var container = AddResultsToContainer(cursor, table.Fields, Args.ReturnValues, Args.SpatialRefefence);

            Marshal.ReleaseComObject(cursor);
            Marshal.ReleaseComObject(table);
//            Marshal.ReleaseComObject(rasterLayer);
            Marshal.ReleaseComObject(featureClass);
            Marshal.ReleaseComObject(FeatureWorkspace);

            Result = container;
        }

        protected virtual ResponseContainer<SearchResult> AddResultsToContainer(ICursor cursor, IFields fields, string[] returnValues, ISpatialReference spatialReference = null)
        {
            var container = new ResponseContainer<SearchResult>
            {
                Results = new List<SearchResult>()
            };

            IRow row;
            while ((row = cursor.NextRow()) != null)
            {
                var feature = new SearchResult
                    {
                        Attributes = new Dictionary<string, object>()
                    };

                for (var i = 0; i < row.Fields.FieldCount; i++)
                {
                    var fieldName = row.Fields.Field[i].Name;
                    if (!returnValues.Contains(fieldName.ToLowerInvariant()))
                    {
                        continue;
                    }

                    feature.Attributes.Add(row.Fields.Field[i].Name, row.Value[i]);
                }

                if (!feature.Attributes.Any())
                {
                    continue;
                }

                container.Results.Add(feature);
                Marshal.ReleaseComObject(row);
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
            var fieldCount = fields.FieldCount;
            string value = null;
            var i = 0;

            try
            {
                while (i <= fieldCount)
                {
                    string property;
                    row.GetPropAndValues(i, out property, out value);

                    if (string.Equals(property, attributeName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return value;
                    }
                    if (attributeName == "VALUE" && property.ToUpperInvariant() == "PIXEL VALUE")
                    {
                        return value;
                    }

                    i = i + 1;
                }
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
            return $"PointInPolygonQueryCommand, Args: {Args}";
        }
    }
}