using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LandOwnership
    {
        public int Xid { get; set; }
        public string Owner { get; set; }
        public string Agency { get; set; }
        public string Admin { get; set; }
        public string Desig { get; set; }
        public string StateLgd { get; set; }
        public string UtLgd { get; set; }
        public string NatlLgd { get; set; }
        public DateTime? EditDate { get; set; }
        public string LabelFederal { get; set; }
        public string LabelState { get; set; }
        public decimal? GisAcres { get; set; }
        public string County { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
