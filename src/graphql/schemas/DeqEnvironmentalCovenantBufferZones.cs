using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DeqEnvironmentalCovenantBufferZones
    {
        public int Xid { get; set; }
        public decimal? FidInstit { get; set; }
        public decimal? Id { get; set; }
        public string Locname { get; set; }
        public string Locstr { get; set; }
        public string Loccity { get; set; }
        public string Derrid { get; set; }
        public string Altfacilit { get; set; }
        public string Facilitype { get; set; }
        public string Loccounty { get; set; }
        public string Locstate { get; set; }
        public string Loczip { get; set; }
        public string Projectman { get; set; }
        public string Maplabel { get; set; }
        public string Division { get; set; }
        public string Branch { get; set; }
        public string Program { get; set; }
        public string Sitedesc { get; set; }
        public string Department { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
