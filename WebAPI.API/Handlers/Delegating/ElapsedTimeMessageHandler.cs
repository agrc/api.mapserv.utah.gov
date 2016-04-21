using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.API.Handlers.Delegating
{
    internal class ElapsedTimeMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            var sw = new Stopwatch();
            sw.Start();

            return base.SendAsync(request, cancellationToken)
                       .ContinueWith(task =>
                       {
                           sw.Stop();
                           task.Result.Headers.Add("X-Elapsed-Time",
                                                   sw.ElapsedMilliseconds.ToString());
                           return task.Result;
                       });
        }
    }
}