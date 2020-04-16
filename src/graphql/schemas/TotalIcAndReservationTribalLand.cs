using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TotalIcAndReservationTribalLand
    {
        public int Xid { get; set; }
        public decimal? Areas { get; set; }
        public decimal? Perimeter { get; set; }
        public decimal? Indncntry { get; set; }
        public decimal? Indncntry1 { get; set; }
        public string Air7Name { get; set; }
        public string Air7Sourc { get; set; }
        public string Air7Scale { get; set; }
        public decimal? Acres { get; set; }
        public decimal? SqMiles { get; set; }
        public string Tname { get; set; }
        public string R8name { get; set; }
        public string BiaClass9 { get; set; }
        public decimal? IndnctryI { get; set; }
        public string Names { get; set; }
        public DateTime? Update { get; set; }
        public decimal? ShapeLeng { get; set; }
        public string Owner { get; set; }
        public string Agency { get; set; }
        public string Admin { get; set; }
        public string Desig { get; set; }
        public string StateLgd { get; set; }
        public string UtLgd { get; set; }
        public string NatlLgd { get; set; }
        public string LabelStat { get; set; }
        public string LabelFede { get; set; }
        public decimal? GisAcres { get; set; }
        public DateTime? EditDate { get; set; }
        public decimal? Type { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
