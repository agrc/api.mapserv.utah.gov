using Ninject.Modules;
using WebAPI.Common.Providers;
using WebAPI.Common.Providers.Urls;

namespace WebAPI.Common.Ninject.Modules
{
    public class DomainLinkModule : NinjectModule
    {
        public override void Load()
        {
#if DEBUG
            
                Bind<IDomainLinkProvider>()
                    .To<LocalDomainLinkProvider>();
#else
            Bind<IDomainLinkProvider>()
                .To<ProductionDomainLinkProvider>();
#endif
        }
    }
}
