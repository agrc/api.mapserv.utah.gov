using System.Collections.Generic;
using HotChocolate;

namespace graphql {
    public class Query {
        public IEnumerable<County> GetCounties() => new List<County> {
                new County{
                    Number = "03",
                    EntityNumber = 2010031010,
                    EntityYear = 2010,
                    Name = "CACHE",
                    Fips = 49005,
                    StatePlaneRegion = "North",
                    PopulationLastCensus = 113307,
                    PopulationCurrentEstimate = 128886
                }
            };
    }
}
