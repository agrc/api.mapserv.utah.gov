using Ninject.Activation;
using Ninject.Modules;
using StackExchange.Redis;

namespace WebAPI.Dashboard.Ninject.Modules
{
        public class RedisModule : NinjectModule
        {
            /// <summary>
            /// Loads the module into the kernel.
            /// </summary>
            public override void Load()
            {
                Bind<ConnectionMultiplexer>()
                    .ToMethod(Init)
                    .InSingletonScope();
            }

            private static ConnectionMultiplexer Init(IContext context)
            {
                return ConnectionMultiplexer.Connect("localhost");
            }
        }
}