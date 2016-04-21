using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Soe.Common.Extensions;
using Soe.Common.Infastructure.Commands;
using WebAPI.Search.Soe.Models;
using WebAPI.Search.Soe.Models.Results;

namespace WebAPI.Search.Soe.Commands
{
    public abstract class QueryCommandBase : Command<ResponseContainer<SearchResult>>
    {
        protected virtual ResponseContainer<SearchResult> AddResultsToContainer(IFeatureCursor fCursor, IFields fields, string[] returnValues, ISpatialReference spatialReference = null)
        {
            var container = new ResponseContainer<SearchResult>
                {
                    Results = new List<SearchResult>()
                };

            IFeature row;
            while ((row = fCursor.NextFeature()) != null)
            {
                var feature = new SearchResult
                    {
                        Attributes = new Dictionary<string, object>()
                    };

                foreach (var attribute in returnValues)
                {
                    var cleantribute = attribute.Trim().ToUpperInvariant();
                    if (cleantribute.Contains("SHAPE@"))
                    {
                        var shape = row.ShapeCopy;

                        if (cleantribute == "SHAPE@ENVELOPE")
                        {
                            shape = shape.Envelope;
                        }

                        feature.Geometry =
                            CommandExecutor.ExecuteCommand(new CreateGraphicFromGeometryCommand(shape, spatialReference));

                        Marshal.ReleaseComObject(shape);
                        continue;
                    }

                    var result = CommandExecutor.ExecuteCommand(new GetValueForFieldCommand(cleantribute, fields, row));

                    feature.Attributes.Add(attribute.Trim(), result);
                }

                container.Results.Add(feature);
            }

            Marshal.ReleaseComObject(fCursor);
            
            return container;
        }

        protected virtual IEnumerable<string> ValidateFields(IFeatureClass featureClass, IEnumerable<string> returnValues)
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
    }
}