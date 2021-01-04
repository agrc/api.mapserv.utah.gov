using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Extensions;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Search {

    public class FormatAttributesCommand : Command<List<SearchResult>> 
    {
        private readonly AttributeStyle _style;
        private readonly IReadOnlyList<SearchResult> _results;
        private readonly IReadOnlyList<string> _returnValues;

        public FormatAttributesCommand(AttributeStyle style, IReadOnlyList<SearchResult> results, IReadOnlyList<string> returnValues = null)
        {
            _style = style;
            _results = results;
            _returnValues = returnValues;
            if (_returnValues is null)
            {
                _returnValues = new List<string>();
            }
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
                            case AttributeStyle.Identical:
                            {
                                if (_returnValues.Any(x => attribute.Key.Equals(x, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    var key = _returnValues.First(x => x.Equals(attribute.Key, StringComparison.InvariantCultureIgnoreCase));

                                    camelDictionary.Add(key, attribute.Value);
                                    continue;
                                }

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