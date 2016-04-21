using System.Linq;
using Raven.Client.Indexes;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Common.Indexes
{
    public class IndexEmail : AbstractIndexCreationTask<Account>
    {
        public IndexEmail()
        {
            Map = xx => from x in xx
                        select new
                        {
                            x.Email
                        };
        }
    }
}