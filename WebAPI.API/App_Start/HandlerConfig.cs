using System.Collections.ObjectModel;
using System.Net.Http;
using Ninject;
using WebAPI.API.App_Start;
using WebAPI.API.Handlers.Delegating;

namespace WebAPI.API
{
    public static class HandlerConfig
    {
        /// <summary>
        ///     Registers the handlers in the order they appear.
        /// </summary>
        /// <param name="messageHandlers">The message handlers.</param>
        public static void RegisterHandlers(Collection<DelegatingHandler> messageHandlers)
        {
            messageHandlers.Add(new SuppressStatusCodeHandler());
            messageHandlers.Add(new ElapsedTimeMessageHandler());
            messageHandlers.Add(App.Kernel.Get<AuthorizeRequestHandler>());
            messageHandlers.Add(new GeometryFormatHandler());
            messageHandlers.Add(App.Kernel.Get<EsriJsonHandler>());
            messageHandlers.Add(App.Kernel.Get<GeoJsonHandler>());
            messageHandlers.Add(new CorsHandler());
            messageHandlers.Add(App.Kernel.Get<ApiLoggingHandler>());
        }
    }
}