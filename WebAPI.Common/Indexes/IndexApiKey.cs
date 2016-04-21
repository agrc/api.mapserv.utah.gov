using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class IndexApiKey : AbstractIndexCreationTask<ApiKey>
    {
        public IndexApiKey()
        {
            Map = keys => from key in keys
                          select new
                          {
                              key.Key
                          };
        }
    }
}
