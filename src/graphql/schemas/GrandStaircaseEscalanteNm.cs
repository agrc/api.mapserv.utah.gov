using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GrandStaircaseEscalanteNm
    {
        public int Xid { get; set; }
        public string NmName { get; set; }
        public string NmState { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
