using System.Collections;
using System.Collections.Generic;
using WebAPI.Domain.ArcServerResponse.Soe;

namespace WebAPI.Domain.ArcServerResponse.MapService
{
    public class QueryResponse : SoeErrorable
    {
        public IEnumerable<QueryResult> Features { get; set; }

        public class QueryResult
        {
            public Geometry Geometry { get; set; }
            public Dictionary<string, object> Attributes { get; set; }
        }

        public class Geometry
        {
            public double X { get; set; }

            public double Y { get; set; }
        }
    }
}