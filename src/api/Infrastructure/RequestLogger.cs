using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Serilog;

namespace api.mapserv.utah.gov.Infrastructure
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> {
        private readonly ILogger _log;

        public RequestLogger(ILogger log) {
            _log = log;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken) {
            var fullname = typeof(TRequest).FullName;
            var space = typeof(TRequest).Namespace.Length + 1;

            var name = fullname.Substring(space, fullname.Length - space);

            _log.Information("Request: {Name} {@Request}", name, request);

            return Task.CompletedTask;
        }
    }
}
