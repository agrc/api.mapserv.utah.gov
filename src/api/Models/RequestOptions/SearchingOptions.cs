using System.ComponentModel;
using AGRC.api.Models.Constants;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Models.RequestOptions {
    [ModelBinder(BinderType = typeof(SearchingOptionsBinder))]
    public class SearchingOptions : ProjectableOptions {
        private string _predicate = "";
        private double _buffer;

        /// <summary>
        ///     The where clause to be evaluated
        /// </summary>
        public string Predicate {
            get => _predicate;
            set => _predicate = value?.ToUpperInvariant();
        }

        /// <summary>
        ///     The coordinate pair representing a point.
        /// <example>
        ///     points:[x,y]
        /// </example>
        /// </summary>
        public string Geometry { get; set; } = "";

        /// <summary>
        ///     The buffer distance in meters. Any valid double less than or equal to 2000
        /// </summary>
        public double Buffer {
            get => _buffer;
            set {
                if (value > 2000) {
                    _buffer = 2000;

                    return;
                }

                _buffer = value;
            }
        }

        /// <summary>
        ///     Determines how attributes will be formatted in the response.
        ///     Data: As stored in the data
        ///     Lower: lower cased eg: lowercase
        ///     Upper: upper cased eg: UPPERCASE
        ///     AliasData: Use the field alias as stored in the data
        ///     AliasLower: lower case the field alias eg: alias
        ///     AliasUpper: upper case the field alias eg: ALIAS
        ///     Default: Data
        /// </summary>
        [DefaultValue(AttributeStyle.Input)]
        public AttributeStyle AttributeStyle { get; set; } = AttributeStyle.Input;
    }
}
