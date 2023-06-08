using System.ComponentModel;
using AGRC.api.Models;
using AGRC.api.Models.Constants;
using AGRC.api.Models.RequestOptionContracts;
using Microsoft.AspNetCore.Mvc;

namespace AGRC.api.Features.Searching;
[ModelBinder(BinderType = typeof(SearchRequestOptionsContractBinder))]
public class SearchRequestOptionsContract : ProjectableOptions {
    private double _buffer;

    /// <summary>
    /// The where clause to be evaluated
    /// </summary>
    /// <example>
    /// name like '%ville'
    /// </example>
    public string? Predicate { get; set; }

    /// <summary>
    /// The coordinate pair representing a point.
    /// </summary>
    /// <example>
    /// points:[x,y] or
    /// {"spatialReference":{"wkid":26912},"x":x,"y":y}
    /// </example>
    public string? Geometry { get; set; } = string.Empty;

    /// <summary>
    /// The buffer distance in meters. Any valid double less than or equal to 2000
    /// </summary>
    /// <example>
    /// 500
    /// </example>
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
    /// </summary>
    // TODO!: v1 this value is lower
    [DefaultValue(AttributeStyle.Input)]
    public AttributeStyle AttributeStyle { get; set; } = AttributeStyle.Input;
}

public sealed class SearchOptions : SearchRequestOptionsContract {
    public SearchOptions(SearchRequestOptionsContract options) {
        AttributeStyle = options.AttributeStyle;
        Buffer = options.Buffer;
        Geometry = options.Geometry;
        Predicate = options.Predicate;
        SpatialReference = options.SpatialReference;
        Format = options.Format;
    }
    public PointWithSpatialReference? Point { get; set; }
}
