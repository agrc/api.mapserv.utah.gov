using System.ComponentModel;
using AGRC.api.Models;
using AGRC.api.Models.Constants;
using AGRC.api.Models.RequestOptionContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace AGRC.api.Features.Searching;
public class SearchRequestOptionsContract : ProjectableOptions {
    private double _buffer = 0;

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
    public string Geometry { get; set; } = string.Empty;

    /// <summary>
    /// The buffer distance in meters. Any valid double less than or equal to 2000
    /// </summary>
    /// <example>
    /// 500
    /// </example>
    public double Buffer {
        get => _buffer;
        set {
            _buffer = Math.Abs(value);

            if (_buffer > 2000) {
                _buffer = 2000;

                return;
            }
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
    // TODO!: v2 this value should be input
    [DefaultValue(AttributeStyle.Lower)]
    public AttributeStyle AttributeStyle { get; set; } = AttributeStyle.Lower;

    public static ValueTask<SearchRequestOptionsContract> BindAsync(HttpContext context) {
        var keyValueModel = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
        keyValueModel.TryGetValue("geometry", out var pointJson);
        keyValueModel.TryGetValue("spatialReference", out var spatialReference);
        keyValueModel.TryGetValue("predicate", out var predicate);
        keyValueModel.TryGetValue("attributeStyle", out var attributeValue);
        keyValueModel.TryGetValue("buffer", out var bufferAmount);

        var attribute = attributeValue.ToString();
        var version = context.GetRequestedApiVersion();

        AttributeStyle attributeStyle;
        if (string.IsNullOrEmpty(attribute)) {
            attributeStyle = Models.Constants.AttributeStyle.Lower;

            if (version > ApiVersion.Default) {
                attributeStyle = Models.Constants.AttributeStyle.Input;
            }
        } else {
            if (!Enum.TryParse(attribute, true, out attributeStyle)) {
                attributeStyle = Models.Constants.AttributeStyle.Lower;

                if (version > ApiVersion.Default) {
                    attributeStyle = Models.Constants.AttributeStyle.Input;
                }
            }
        }

        var wkid = 26912;
        if (!string.IsNullOrEmpty(spatialReference)) {
            if (!int.TryParse(spatialReference, out wkid)) {
                // reset to default
                wkid = 26912;
            }
        }

        var result = new SearchRequestOptionsContract {
            Predicate = predicate,
            Geometry = pointJson,
            Buffer = Convert.ToDouble(bufferAmount),
            AttributeStyle = attributeStyle,
            SpatialReference = wkid
        };

        return new ValueTask<SearchRequestOptionsContract>(result);
    }
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
