using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LawEnforcement
    {
        public int Xid { get; set; }
        public string Id { get; set; }
        public string Secclass { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Zipp4 { get; set; }
        public string County { get; set; }
        public string Fips { get; set; }
        public string Directions { get; set; }
        public string Emergtitle { get; set; }
        public string Emergtel { get; set; }
        public string Emergext { get; set; }
        public DateTime? Contdate { get; set; }
        public string Conthow { get; set; }
        public DateTime? Geodate { get; set; }
        public string Geohow { get; set; }
        public string Naicscode { get; set; }
        public string Naicsdescr { get; set; }
        public string Geolinkid { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public string StVendor { get; set; }
        public string StVersion { get; set; }
        public string Geoprec { get; set; }
        public string Phoneloc { get; set; }
        public string QcQa { get; set; }
        public string Typeofagen { get; set; }
        public string InmateMen { get; set; }
        public string InmateWom { get; set; }
        public string InmateJuv { get; set; }
        public string InmateCoe { get; set; }
        public string Ci { get; set; }
        public Point Shape { get; set; }
    }
}
