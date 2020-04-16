using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class NationalGrid
    {
        public int Xid { get; set; }
        public string Usng { get; set; }
        public string Grid1mil { get; set; }
        public string Grid100k { get; set; }
        public string Easting { get; set; }
        public string Northing { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
