using System.Linq;
using Raven.Client.Documents.Session;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GenerateUniqueConfirmationKeyCommand : Command<string>
    {
        public GenerateUniqueConfirmationKeyCommand(IDocumentSession session)
        {
            Session = session;
        }

        public IDocumentSession Session { get; set; }

        public override string ToString()
        {
            return "GenerateUniqueConfirmationKeyCommand";
        }

        protected override void Execute()
        {
            var confirmationKey = CommandExecutor.ExecuteCommand(new GenerateKeyCommand());

            while (Session.Query<Account, IndexEmailConfirmationKey>().Any(x => x.Confirmation.Key == confirmationKey))
            {
                confirmationKey = CommandExecutor.ExecuteCommand(new GenerateKeyCommand());
            }

            Result = confirmationKey;
        }
    }
}