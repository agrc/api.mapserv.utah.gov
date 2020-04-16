using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MillardCountyParcelsLir
    {
        public int Xid { get; set; }
        public string CountyName { get; set; }
        public string CountyId { get; set; }
        public string AssessorSrc { get; set; }
        public string BoundarySrc { get; set; }
        public string Disclaimer { get; set; }
        public DateTime? CurrentAsof { get; set; }
        public string ParcelId { get; set; }
        public string SerialNum { get; set; }
        public string ParcelAdd { get; set; }
        public string ParcelCity { get; set; }
        public string TaxexemptType { get; set; }
        public string TaxDistrict { get; set; }
        public decimal? TotalMktValue { get; set; }
        public decimal? LandMktValue { get; set; }
        public decimal? ParcelAcres { get; set; }
        public string PropClass { get; set; }
        public string PrimaryRes { get; set; }
        public string HouseCnt { get; set; }
        public string SubdivName { get; set; }
        public decimal? BldgSqft { get; set; }
        public string BldgSqftInfo { get; set; }
        public decimal? FloorsCnt { get; set; }
        public string FloorsInfo { get; set; }
        public decimal? BuiltYr { get; set; }
        public decimal? EffbuiltYr { get; set; }
        public string ConstMaterial { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
