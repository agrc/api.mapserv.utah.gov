using Ninject.Modules;
using WebAPI.Common.Authentication.Forms;

namespace WebAPI.Common.Ninject.Modules
{
    public class FormsAuthModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFormsAuthentication>().To<FormsAuth>();
        }
    }
}