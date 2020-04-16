using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AquiferRechargeDischargeAreas
    {
        public int Xid { get; set; }
        public string Zone { get; set; }
        public string Studyarea { get; set; }
        public string Agency { get; set; }
        public string Author { get; set; }
        public string Pubyear { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
