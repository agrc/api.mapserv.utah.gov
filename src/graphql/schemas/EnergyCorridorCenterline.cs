using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnergyCorridorCenterline
    {
        public int Xid { get; set; }
        public string Comments { get; set; }
        public string Designated { get; set; }
        public string Desiguse { get; set; }
        public string Name { get; set; }
        public decimal? Widthfeet { get; set; }
        public string StateAbbr { get; set; }
        public string SmaCode { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
