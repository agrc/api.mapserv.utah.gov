using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqAssessmentUnits
    {
        public int Xid { get; set; }
        public string AuName { get; set; }
        public string AuId { get; set; }
        public string BenClass { get; set; }
        public string Unit { get; set; }
        public string AuDescrip { get; set; }
        public decimal? Miles2008 { get; set; }
        public decimal? AcreGis { get; set; }
        public string Protected { get; set; }
        public string Cat2006 { get; set; }
        public string Status2006 { get; set; }
        public string Cause2006 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
