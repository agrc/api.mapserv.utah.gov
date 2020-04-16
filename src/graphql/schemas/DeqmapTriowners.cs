using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapTriowners
    {
        public int Xid { get; set; }
        public decimal? Ownlinkkey { get; set; }
        public string Derrid { get; set; }
        public string Cimid { get; set; }
        public string OwnerName { get; set; }
        public string Ownerphone { get; set; }
        public string Owneremail { get; set; }
        public string OwnerAddr { get; set; }
        public string OwnerCity { get; set; }
        public string OwnerCnty { get; set; }
        public string Ownerstate { get; set; }
        public string OwnerZip { get; set; }
        public string OwnerZip4 { get; set; }
    }
}
