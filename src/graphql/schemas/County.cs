using HotChocolate;

namespace graphql {
    public class County {
        [GraphQLNonNullType]
        public string Number { get; set; }
        public int EntityNumber { get; set; }
        public int EntityYear { get; set; }
        public int Fips { get; set; }
        [GraphQLNonNullType]
        public string Name { get; set; }
        public int PopulationCurrentEstimate { get; set; }
        public int PopulationLastCensus { get; set; }
        public string StatePlaneRegion { get; set; }
    }
}
