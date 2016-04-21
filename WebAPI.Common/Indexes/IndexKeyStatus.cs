using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class IndexKeyStatus : AbstractIndexCreationTask<ApiKey>
    {
        public IndexKeyStatus()
        {
            Map = keys => from key in keys
                          where !key.Deleted && key.ApiKeyStatus == ApiKey.KeyStatus.Active
                          select new
                          {
                              key.ApiKeyStatus
                          };
        }
    }
}