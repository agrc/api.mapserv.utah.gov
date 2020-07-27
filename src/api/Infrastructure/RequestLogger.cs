using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Serilog;

namespace AGRC.api.Infrastructure
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> {
        private readonly ILogger _log;

        public RequestLogger(ILogger log) {
            _log = log?.ForContext<MediatR.IMediator>();
        }

        public Task Process(TRequest request, CancellationToken cancellationToken) {
            var name = typeof(TRequest).FullName;

            _log.Information("processing: {Name} {@Request}", name, request);

            return Task.CompletedTask;
        }
    }
}
