using System.Collections.Generic;

namespace WebAPI.Domain.ArcServerResponse.Soe
{
    public class SearchResponse : SoeErrorable
    {
        public List<SearchResult> Results { get; set; }
        
        public class SearchResult
        {
            public string Geometry { get; set; }
            public Dictionary<string,object> Attributes { get; set; }
        }
    }
}