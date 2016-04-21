using System;
using System.Collections.Generic;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Search {

    public class FormatAttributesCommand : Command<List<SearchResult>> 
    {
        private readonly AttributeStyle _style;
        private readonly IReadOnlyList<SearchResult> _results;

        public FormatAttributesCommand(AttributeStyle style, IReadOnlyList<SearchResult> results)
        {
            _style = style;
            _results = results;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, Style: {1}", "FormatAttributesCommand", _style);
        }

        protected override void Execute()
        {
            var resultsFormatted = new List<SearchResult>(_results.Count);

            foreach (var searchResult in _results)
            {
                var camelDictionary = new Dictionary<string, object>(searchResult.Attributes.Count);
                foreach (var attribute in searchResult.Attributes)
                {
                    switch (_style)
                    {
                        case AttributeStyle.Camel:
                            {
                                camelDictionary.Add(attribute.Key.ToCamelCase(), attribute.Value);
                                continue;
                            }
                            case AttributeStyle.Lower:
                            {
                                camelDictionary.Add(attribute.Key.ToLowerInvariant(), attribute.Value);
                                continue;
                            }
                            case AttributeStyle.Upper:
                            {
                                camelDictionary.Add(attribute.Key.ToUpperInvariant(), attribute.Value);
                                continue;
                            }
                    }
                }

                resultsFormatted.Add(new SearchResult
                {
                    Attributes = camelDictionary,
                    Geometry = searchResult.Geometry
                });
            }

            Result = resultsFormatted;
        }
    }
}