using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CensusEdges2010
    {
        public int Xid { get; set; }
        public string Statefp { get; set; }
        public string Countyfp { get; set; }
        public decimal? Tlid { get; set; }
        public decimal? Tfidl { get; set; }
        public decimal? Tfidr { get; set; }
        public string Mtfcc { get; set; }
        public string Fullname { get; set; }
        public string Smid { get; set; }
        public string Lfromadd { get; set; }
        public string Ltoadd { get; set; }
        public string Rfromadd { get; set; }
        public string Rtoadd { get; set; }
        public string Zipl { get; set; }
        public string Zipr { get; set; }
        public string Featcat { get; set; }
        public string Hydroflg { get; set; }
        public string Railflg { get; set; }
        public string Roadflg { get; set; }
        public string Olfflg { get; set; }
        public string Passflg { get; set; }
        public string Divroad { get; set; }
        public string Exttyp { get; set; }
        public string Ttyp { get; set; }
        public string Deckedroad { get; set; }
        public string Artpath { get; set; }
        public string Persist { get; set; }
        public string Gcseflg { get; set; }
        public string Offsetl { get; set; }
        public string Offsetr { get; set; }
        public decimal? Tnidf { get; set; }
        public decimal? Tnidt { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
