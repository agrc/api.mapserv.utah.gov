using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DominantVegetation
    {
        public int Xid { get; set; }
        public string Code { get; set; }
        public string Dom { get; set; }
        public string Ss1 { get; set; }
        public string Ss2 { get; set; }
        public string Ss3 { get; set; }
        public string Ss4 { get; set; }
        public string Ss5 { get; set; }
        public string Ss6 { get; set; }
        public string Ss7 { get; set; }
        public string Ss8 { get; set; }
        public string Ss9 { get; set; }
        public string Ss10 { get; set; }
        public string Lab { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
