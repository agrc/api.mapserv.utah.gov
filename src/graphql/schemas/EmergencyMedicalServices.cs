using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EmergencyMedicalServices
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string Fips { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public string Frst { get; set; }
        public Point Shape { get; set; }
    }
}
