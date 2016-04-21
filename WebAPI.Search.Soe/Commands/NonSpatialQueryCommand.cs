using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using Soe.Common.Extensions;
using WebAPI.Search.Soe.Models;

namespace WebAPI.Search.Soe.Commands
{
    public class NonSpatialQueryCommand : QueryCommandBase
    {
        public NonSpatialQueryCommand(IFeatureWorkspace featureWorkspace, QueryArgs args)
        {
            FeatureWorkspace = featureWorkspace;
            Args = args;
        }

        public IFeatureWorkspace FeatureWorkspace { get; set; }
        public QueryArgs Args { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Args: {1}", "NonSpatialQueryCommand", Args);
        }

        protected override void Execute()
        {
            var featureClass = FeatureWorkspace.OpenFeatureClass(Args.FeatureClass.Trim());

            var errors = ValidateFields(featureClass, Args.ReturnValues);

            if (errors.Any())
            {
                ErrorMessage = "{0} does not contain an attribute {1}. Check your spelling."
                    .With(Args.FeatureClass, string.Join(" or ", errors));

                return;
            }

            var filter = new QueryFilterClass
                {
                    WhereClause = Args.WhereClause
                };

            var fCursor = featureClass.Search(filter, true);

            var fields = featureClass.Fields;

            var container = AddResultsToContainer(fCursor, fields, Args.ReturnValues, Args.SpatialRefefence);

            Result = container;
        }
    }
}