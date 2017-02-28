using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using WebAPI.API.Commands.Spatial;
using WebAPI.Common.Executors;
using WebAPI.Common.Providers;

namespace WebAPI.API.Handlers.Delegating
{
    public class EsriJsonHandler : DelegatingHandler
    {
        [Inject]
        public HttpContentProvider HttpContentProvider { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(response =>
            {
                IEnumerable<string> formats, types;

                var requestContent = HttpContentProvider.GetRequestContent(request);

                if (!requestContent.Headers.TryGetValues("X-Format", out formats))
                {
                    return response.Result;
                }

                if (response.Result == null || response.Result.Content == null || !formats.Contains("esrijson"))
                {
                    return response.Result;
                }

                var responseContent = HttpContentProvider.GetResponseContent(response.Result);

                if (!responseContent.Headers.TryGetValues("X-Type", out types))
                {
                    return response.Result;
                }

                var typeName = types.FirstOrDefault();

                if (typeName == null)
                {
                    return response.Result;
                }

                var assembly = typeof (Domain.ApiResponses.GeocodeAddressResult).Assembly;
                var type = assembly.GetType(typeName);

                try
                {
                    var result = responseContent.ReadAsAsync(type)
                                .ContinueWith(y =>
                                {
                                    HttpStatusCode status;
                                    var graphic = CommandExecutor.ExecuteCommand(new CreateGraphicFromCommand(y.Result));

                                    if (graphic == null)
                                    {
                                        return response.Result;
                                    }

                                    if(!Enum.TryParse(graphic.Status.ToString(), out status))
                                    {
                                        status = HttpStatusCode.OK;
                                    }

                                    return request.CreateResponse(status, graphic);
                                });

                    return result.Result;
                }
                catch (AggregateException)
                {
                    
                }

                return response.Result;
            });
        }
    }
}