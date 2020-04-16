using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapTrichem
    {
        public int Xid { get; set; }
        public string Derrid { get; set; }
        public string Cimid { get; set; }
        public decimal? Trilnkkey { get; set; }
        public string Chemical { get; set; }
        public decimal? AveAmount { get; set; }
        public string FormSubm { get; set; }
        public string Facprochem { get; set; }
        public string Facimpchem { get; set; }
        public string ManOnsite { get; set; }
        public string ManSale { get; set; }
        public string ManProd { get; set; }
        public string ManImpuri { get; set; }
        public string ProReact { get; set; }
        public string ProFormul { get; set; }
        public string ProArticl { get; set; }
        public string ProRepack { get; set; }
        public string ProImpuri { get; set; }
        public string OtherProa { get; set; }
        public string OtherManu { get; set; }
        public string OtherUse { get; set; }
        public string TradeSecr { get; set; }
        public string ProdRatio { get; set; }
        public decimal? QuantRel { get; set; }
        public string Revision { get; set; }
        public string ActRevisi { get; set; }
        public decimal? Trichemkey { get; set; }
    }
}
