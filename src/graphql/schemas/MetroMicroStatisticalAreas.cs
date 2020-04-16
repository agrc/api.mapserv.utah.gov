using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MetroMicroStatisticalAreas
    {
        public int Xid { get; set; }
        public string Csafp { get; set; }
        public string Cbsafp { get; set; }
        public string Name { get; set; }
        public string Namelsad { get; set; }
        public string Lsad { get; set; }
        public string Memi { get; set; }
        public string Mtfcc { get; set; }
        public string Funcstat { get; set; }
        public decimal? Aland { get; set; }
        public decimal? Awater { get; set; }
        public string Intptlat { get; set; }
        public string Intptlon { get; set; }
        public string Partflg { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
