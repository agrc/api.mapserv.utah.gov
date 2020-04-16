using HotChocolate.Types;
using NetTopologySuite.Geometries;

namespace graphql {
    public class SpatialType : ObjectType<Geometry> {

        protected override void Configure(IObjectTypeDescriptor<Geometry> descriptor) {
            descriptor.BindFieldsExplicitly();
            // descriptor.Field(f => f.AsText()).Name("wkt");
        }
    }
}
