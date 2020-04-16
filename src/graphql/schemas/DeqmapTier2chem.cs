using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapTier2chem
    {
        public int Xid { get; set; }
        public string Derrid { get; set; }
        public string Cimid { get; set; }
        public decimal? Chemlnkkey { get; set; }
        public string Chemical { get; set; }
        public decimal? AveAmount { get; set; }
        public string Tradsecret { get; set; }
        public string PureSub { get; set; }
        public string MixSub { get; set; }
        public string SolidSub { get; set; }
        public string LiquidSub { get; set; }
        public string GasSub { get; set; }
        public string EhsSub { get; set; }
        public string EhsName { get; set; }
        public string FireHaz { get; set; }
        public string PresHaz { get; set; }
        public string ReactHaz { get; set; }
        public string ImmedHaz { get; set; }
        public string DelayHaz { get; set; }
        public decimal? Daysonsite { get; set; }
        public decimal? ChemKey { get; set; }
    }
}
