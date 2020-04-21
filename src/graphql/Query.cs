using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using graphql.filters;
using HotChocolate;
using HotChocolate.Types;

namespace graphql {

    public class Query {

        [UseSelection]
        // [UseFiltering]
        public async Task<IQueryable<schemas.CountyBoundaries>> GetCounties([Service]CloudSqlContext context, [Service]IHttpClientFactory clientFactory, GeocodingFilter geocode) {
            if (!geocode.HasFilter()) {
                return context.CountyBoundaries;
            };

            var point = await geocode.GeocodeAsync(clientFactory.CreateClient("geocoder"));

            if (point is null) {
                throw new System.Exception("Address not found");
            }

            var result = context.CountyBoundaries.Where(x => x.Shape.Contains(point));

            return result;
        }

        // [UsePaging]
        [UseSelection]
        [UseFiltering]
        public IQueryable<schemas.MunicipalBoundaries> GetMunis([Service]CloudSqlContext context) => context.MunicipalBoundaries;
    }
}
