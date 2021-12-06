using Ninject.Activation;
using Ninject.Modules;
using Raven.Client.Documents;
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
        Urls = new[] { "http://127.0.0.1:3000" },
        Database = "WSUT"
      }.Initialize();

      RavenConfig.Register(typeof(IndexEmailConfirmationKey), documentStore);

      return documentStore;
    }
  }
}
