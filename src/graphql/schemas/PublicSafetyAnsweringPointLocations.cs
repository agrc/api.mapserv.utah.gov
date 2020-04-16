using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PublicSafetyAnsweringPointLocations
    {
        public int Xid { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string PsapName { get; set; }
        public string PhoneNumber { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string DpsPsapName { get; set; }
        public decimal? PsapType { get; set; }
        public Point Shape { get; set; }
    }
}
