using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthCareFacilities
    {
        public int Xid { get; set; }
        public string Factype { get; set; }
        public decimal? FacilityId { get; set; }
        public string Facid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Telephone { get; set; }
        public string Type { get; set; }
        public string County { get; set; }
        public string Telehealth { get; set; }
        public string FunctionalCategory { get; set; }
        public Point Shape { get; set; }
    }
}
