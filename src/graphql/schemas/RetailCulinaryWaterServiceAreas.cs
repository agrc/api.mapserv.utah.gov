using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class RetailCulinaryWaterServiceAreas
    {
        public int Xid { get; set; }
        public string Wrename { get; set; }
        public string Wrname { get; set; }
        public string Dwname { get; set; }
        public string Systemtype { get; set; }
        public decimal? Waterresid { get; set; }
        public decimal? Wrid { get; set; }
        public string Dwsysnum { get; set; }
        public string Wrlink { get; set; }
        public string Wholesaler { get; set; }
        public string Label { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Basin { get; set; }
        public string Subarea { get; set; }
        public string Subareanam { get; set; }
        public string Lanum { get; set; }
        public string Laname { get; set; }
        public decimal? Endyear { get; set; }
        public string Datasource { get; set; }
        public DateTime? Sourcedate { get; set; }
        public string Editor { get; set; }
        public DateTime? Editdate { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
