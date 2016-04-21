using System.Linq;
using Raven.Client;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Commands.Key
{
    public class CountApiInfosForUserQuery : Query<int>
    {
        protected Account User { get; set; }

        public CountApiInfosForUserQuery(IDocumentSession session, Account account)
            : base(session)
        {
            User = account;
        }

        protected override void Execute()
        {
            Result = Session.Query<ApiKey, IndexKeyStatus>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Count(x => x.AccountId == User.Id);
        }

        public override string ToString()
        {
            return string.Format("{0}, User: {1}", "CountApiInfosForUserQuery", User);
        }
    }
}