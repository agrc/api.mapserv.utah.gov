using System.Linq;
using Raven.Client;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GenerateUniqueApiKeyCommand : Command<string>
    {
        public GenerateUniqueApiKeyCommand(IDocumentSession session)
        {
            Session = session;
        }

        public IDocumentSession Session { get; set; }

        public override string ToString()
        {
            return "GenerateUniqueApiKeyCommand";
        }

        protected override void Execute()
        {
            var apiKey = CommandExecutor.ExecuteCommand(new GenerateKeyCommand());

            while (Session.Query<ApiKey, IndexApiKey>().Any(x => x.Key == apiKey))
            {
                apiKey = CommandExecutor.ExecuteCommand(new GenerateKeyCommand());
            }

            Result = apiKey;
        }
    }
}