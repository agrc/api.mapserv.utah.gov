using System.Linq;
using Raven.Client.Documents.Indexes;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Common.Indexes
{
    public class IndexEmailConfirmationKey : AbstractIndexCreationTask<Account>
    {
        public IndexEmailConfirmationKey()
        {
            Map = keys => from key in keys
                          select new
                          {
                              Confirmation_Key = key.Confirmation.Key
                          };

            Index(rr => rr.Confirmation.Key, FieldIndexing.Default);
        }
    }
}