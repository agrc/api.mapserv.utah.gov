using Ninject.Activation;
using Ninject.Modules;
using Raven.Client;
using Raven.Client.Document;
using WebAPI.Common.Indexes;

namespace WebAPI.Dashboard.Ninject.Modules
{
    public class RavenModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentStore>()
                .ToMethod(InitDocStore)
                .InSingletonScope();
        }

        private static IDocumentStore InitDocStore(IContext context)
        {
            var documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenDb"
            }.Initialize();

            RavenConfig.Register(typeof(IndexEmailConfirmationKey), documentStore);

            return documentStore;
        }
    }
}