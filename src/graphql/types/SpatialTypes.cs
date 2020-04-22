using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Types;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace graphql.types {
    public class PointType : ObjectType<Point> {

        protected override void Configure(IObjectTypeDescriptor<Point> descriptor) {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.AsText()).Name("wkt");
        }
    }

    public class MultiPolygonType : ObjectType<MultiPolygon> {

        protected override void Configure(IObjectTypeDescriptor<MultiPolygon> descriptor) {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.Centroid.AsText()).Name("centroid");
            descriptor.Field(f => f.Envelope.AsText()).Name("envelope");
        }
    }

    public class MultiLineType : ObjectType<MultiLineString> {

        protected override void Configure(IObjectTypeDescriptor<MultiLineString> descriptor) {
            descriptor.BindFieldsExplicitly();
            descriptor.Field(f => f.Centroid.AsText()).Name("centroid");
            descriptor.Field(f => f.Envelope.AsText()).Name("envelope");
        }
    }

    // public class GeoScalar : ScalarType<Geometry> {
    //     private AnyType _anyType = new AnyType();

    //     public GeoScalar() : base("Geometry") {
    //     }

    //     public override bool IsInstanceOfType(IValueNode literal) {
    //         if (literal == null)
    //             throw new ArgumentNullException(nameof(literal));

    //         return literal is ObjectValueNode
    //                || literal is NullValueNode;
    //     }

    //     public override object ParseLiteral(IValueNode literal) {
    //         if (literal == null)
    //             throw new ArgumentNullException(nameof(literal));

    //         if (literal is NullValueNode)
    //             return null;

    //         if (literal is ObjectValueNode obj) {
    //             var objStr = QuerySyntaxSerializer.Serialize(literal);
    //             var reader = new GeoJsonReader();
    //             var geoObject = reader.Read<Geometry>(objStr);
    //             if (geoObject == null) {
    //                 var error = new ErrorBuilder()
    //                     .SetCode("INVALID_ARG")
    //                     .SetMessage($"Couldn't convert {literal} to GeoJson")
    //                     .AddLocation(literal.Location.Line, literal.Location.Column)
    //                     .Build();

    //                 throw new QueryException(error);
    //             }

    //             return geoObject;
    //         }

    //         throw new ArgumentException("The point type can only parse object literals", nameof(literal));
    //     }

    //     public override IValueNode ParseValue(object value) {
    //         if (value == null) {
    //             return new NullValueNode(null);
    //         }

    //         if (value is Geometry) {
    //             var writer = new GeoJsonWriter();

    //             var objStr = writer.Write(value);

    //             // turn json back into object
    //             var obj = JsonSerializer.Deserialize<object>(objStr);

    //             // convert it to graphql node tree
    //             var nodeTree = _anyType.ParseValue(obj);

    //             return nodeTree;
    //         }

    //         throw new ArgumentException("The specified value has to be a Geometry Type");
    //     }

    //     public override object Serialize(object value) {
    //         if (value == null) {
    //             return null;
    //         }

    //         if (value is Geometry geo) {
    //             // We have to serialize and deserialize the object to remove circular references
    //             // from the original Geometry object
    //             var writer = new GeoJsonWriter();
    //             var geoStr = writer.Write(geo);

    //             var reader = new GeoJsonReader();
    //             var deObj = reader.Read<object>(geoStr);

    //             // This will throw an exception, since it's not a .net native type
    //             return deObj;
    //         }

    //         throw new ArgumentException(
    //             "The specified value cannot be serialized by the StringType.");
    //     }
    // }
}
