using System;
using System.Configuration;
using System.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Dashboard.Commands.Password;

namespace WebAPI.Dashboard
{
    public class RavenConfig
    {
        public static void Register(Type type, IDocumentStore documentStore)
        {
            RegisterIndexes(type, documentStore);
            SeedDatabase(documentStore);
        }

        private static void RegisterIndexes(Type type, IDocumentStore documentStore)
        {
            IndexCreation.CreateIndexes(type.Assembly, documentStore);
        }

        private static void SeedDatabase(IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                var adminUser = new Account
                {
                    CodingExperience = 10,
                    Company = "AGRC",
                    Confirmation = new EmailConfirmation(ConfigurationManager.AppSettings["api_explorer_api_key"])
                    {
                        ConfirmationDate = DateTime.UtcNow,
                        Confirmed = true,
                        TimesSent = 0
                    },
                    ContactRoute = "email",
                    Email = ConfigurationManager.AppSettings["admin_email"],
                    FirstName = "AGRC",
                    JobCategory = "Geography",
                    JobTitle = "Government",
                    KeyQuota = new KeyQuota(1)
                    {
                        KeysUsed = 1
                    },
                    LastName = "AGRC",
                    Password = CommandExecutor.ExecuteCommand(new HashPasswordCommand(ConfigurationManager.AppSettings["admin_password"])).Result
                };

                if (!session.Query<Account>().Any(x => x.Email == ConfigurationManager.AppSettings["admin_email"]))
                {
                    session.Store(adminUser);

                    var explorerKey = new ApiKey(ConfigurationManager.AppSettings["api_explorer_api_key"])
                    {
                        AccountId = adminUser.Id,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        CreatedAtTicks = DateTime.UtcNow.Ticks,
                        Deleted = false,
                        IsMachineName = false,
                        Pattern = ".*/beta/webapi/*",
                        RegexPattern = ".*/beta/webapi/*",
                        Type = ApiKey.ApplicationType.Browser
                    };

                    session.Store(explorerKey);
                }

                if (!session.Query<AdminContainer>().Any())
                {
                    session.Store(new AdminContainer(ConfigurationManager.AppSettings["admin_email"].Split(';')));
                }

                session.SaveChanges();
            }
        }
    }
}