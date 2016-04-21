using System.Collections.Generic;

namespace WebAPI.Common.Models.Raven.Whitelist
{
    public class WhitelistContainer
    {
        public WhitelistContainer()
        {
            Items = new List<WhitelistItems>();
        }

        public List<WhitelistItems> Items { get; set; }
    }
}