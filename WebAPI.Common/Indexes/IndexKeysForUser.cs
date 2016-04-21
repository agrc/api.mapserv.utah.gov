using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class IndexKeysForUser : AbstractIndexCreationTask<ApiKey>
    {
        public IndexKeysForUser()
        {
            Map = _ => from x in _
                       select new
                       {
                           x.AccountId,
                           x.ApiKeyStatus,
                           x.Deleted,
                           x.Key
                       };
        }
    }
}