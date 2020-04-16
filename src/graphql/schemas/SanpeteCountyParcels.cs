using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SanpeteCountyParcels
    {
        public int Xid { get; set; }
        public decimal? Fips { get; set; }
        public string ParcelId { get; set; }
        public string ParcelAdd { get; set; }
        public string ParcelCity { get; set; }
        public string ParcelZip { get; set; }
        public string OwnType { get; set; }
        public string Recorder { get; set; }
        public DateTime? Parcelscur { get; set; }
        public DateTime? Parcelsrec { get; set; }
        public DateTime? Parcelspub { get; set; }
        public string Parcelyear { get; set; }
        public string Parcelnotes { get; set; }
        public string CoparcelUrl { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
