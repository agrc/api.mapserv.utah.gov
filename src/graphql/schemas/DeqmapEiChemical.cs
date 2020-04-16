using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapEiChemical
    {
        public int Xid { get; set; }
        public decimal? Derrid { get; set; }
        public string Chemical { get; set; }
        public string ChemicalOther { get; set; }
        public string Amount { get; set; }
        public string AmountType { get; set; }
        public string AmountOther { get; set; }
    }
}
