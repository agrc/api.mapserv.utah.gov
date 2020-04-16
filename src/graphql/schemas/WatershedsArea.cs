using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WatershedsArea
    {
        public int Xid { get; set; }
        public string Huc8 { get; set; }
        public string Huc10 { get; set; }
        public string Huc12 { get; set; }
        public decimal? Acres { get; set; }
        public string States { get; set; }
        public decimal? NcontrbA { get; set; }
        public string Hu10Ds { get; set; }
        public string Hu10Name { get; set; }
        public string Hu10Mod { get; set; }
        public string Hu10Type { get; set; }
        public string Hu12Ds { get; set; }
        public string Hu12Name { get; set; }
        public string Hu12Mod { get; set; }
        public string Hu12Type { get; set; }
        public string Hu8Name { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
