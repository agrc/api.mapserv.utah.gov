using System.Linq;
using Raven.Client.Documents.Session;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Queries
{
    public class CountApiInfosForUserQuery : Query<int>
    {
        public CountApiInfosForUserQuery(IDocumentSession session, Account account)
            : base(session)
        {
            User = account;
        }

        protected Account User { get; set; }

        protected override void Execute()
        {
            Result = Session.Query<ApiKey, IndexKeysForUser>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .Count(x => x.ApiKeyStatus == ApiKey.KeyStatus.Active &&
                                        !x.Deleted &&
                                        x.AccountId == User.Id);
        }

        public override string ToString()
        {
            return string.Format("{0}, User: {1}", "CountApiInfosForUserQuery", User);
        }
    }
}