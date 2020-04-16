using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BoatRamps
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string WaterBody { get; set; }
        public string Owner { get; set; }
        public string Agency { get; set; }
        public string Admin { get; set; }
        public string Desig { get; set; }
        public string StateLgd { get; set; }
        public string Vessels { get; set; }
        public string Featureid { get; set; }
        public Point Shape { get; set; }
    }
}
