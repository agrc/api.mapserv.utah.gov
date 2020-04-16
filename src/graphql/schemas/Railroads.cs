using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Railroads
    {
        public int Xid { get; set; }
        public string Railroad { get; set; }
        public string Passenger { get; set; }
        public string State { get; set; }
        public string Layer { get; set; }
        public decimal? Fips5 { get; set; }
        public string Military { get; set; }
        public string Abandoned { get; set; }
        public string OhElec { get; set; }
        public string Operator { get; set; }
        public string Type { get; set; }
        public string Iscommuter { get; set; }
        public string Coarse { get; set; }
        public string Mainline { get; set; }
        public string Division { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
