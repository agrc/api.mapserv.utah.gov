using System.Linq;
using HotChocolate;
using HotChocolate.Types;

namespace graphql {
    public class Query {
        // [UsePaging]
        [UseSelection]
        [UseFiltering]
        // [UseSorting]
        public IQueryable<schemas.CountyBoundaries> GetCounties([Service]CloudSqlContext context) => context.CountyBoundaries;

        [UseSelection]
        [UseFiltering]
        public IQueryable<schemas.MunicipalBoundaries> GetMunis([Service]CloudSqlContext context) => context.MunicipalBoundaries;
    }
}
