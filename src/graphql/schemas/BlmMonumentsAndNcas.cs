using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmMonumentsAndNcas
    {
        public int Xid { get; set; }
        public string NlcsId { get; set; }
        public string NlcsName { get; set; }
        public string CasefileNo { get; set; }
        public string NlcsType { get; set; }
        public string AdminSt { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
