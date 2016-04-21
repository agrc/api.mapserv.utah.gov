using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Extensions;
using WebAPI.Search.Soe.Models;
using WebAPI.Search.Soe.Models.Results;

namespace WebAPI.Search.Soe.Commands
{
    public class PointInPolygonQueryCommand : QueryCommandBase
    {
        private readonly ResponseContainer<SearchResult> _container = new ResponseContainer<SearchResult>();

        public PointInPolygonQueryCommand(IFeatureWorkspace featureWorkspace, SpatialQueryArgs args)
        {
            FeatureWorkspace = featureWorkspace;
            Args = args;

            _container.Results = new List<SearchResult>();
        }

        public IFeatureWorkspace FeatureWorkspace { get; set; }
        public SpatialQueryArgs Args { get; set; }

        protected override void Execute()
        {
            IFeatureClass featureClass;
            try
            {
                featureClass = FeatureWorkspace.OpenFeatureClass(Args.FeatureClass.Trim());
            }
            catch (COMException ex)
            {
                ErrorMessage =
                    "{0} is not a spatial feature class and cannot be queried with a geometry. Result: {1}".With(Args.FeatureClass, ex.HResult);

                return;
            }

            var errors = ValidateFields(featureClass, Args.ReturnValues);

            if (errors.Any())
            {
                ErrorMessage = "{0} does not contain an attribute {1}. Check your spelling."
                    .With(Args.FeatureClass, string.Join(" or ", errors));

                return;
            }

            if (Args.Shape == null)
            {
                ErrorMessage = "The point used to do the query is fouled up. Check the syntax point:[x,y]";

                return;
            }

            var spatFilter = new SpatialFilterClass
                {
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                    WhereClause = Args.WhereClause,
                    Geometry = Args.Shape
                };

            var fCursor = featureClass.Search(spatFilter, true);

            var fields = featureClass.Fields;

            var container = AddResultsToContainer(fCursor, fields, Args.ReturnValues, Args.SpatialRefefence);

            Marshal.ReleaseComObject(fields);
            Marshal.ReleaseComObject(spatFilter);
            Marshal.ReleaseComObject(featureClass);
            Marshal.ReleaseComObject(FeatureWorkspace);

            Result = container;
        }

        public override string ToString()
        {
            return string.Format("{0}, Args: {1}", "PointInPolygonQueryCommand", Args);
        }
    }
}