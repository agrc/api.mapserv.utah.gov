using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmMonumentsAndNcasHistoric
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string CasefileNo { get; set; }
        public string NlcsType { get; set; }
        public string AdminSt { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
