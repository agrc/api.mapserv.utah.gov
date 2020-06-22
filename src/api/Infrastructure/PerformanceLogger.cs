using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Infrastructure {
    public class PerformanceLogger<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
        private readonly Stopwatch _timer;
        private readonly ILogger _log;

        public PerformanceLogger(ILogger log) {
            _timer = new Stopwatch();
            _log = log?.ForContext<PerformanceLogger<TRequest, TResponse>>();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500) {
                var name = typeof(TRequest).Name;

                _log.Warning("Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}", name, _timer.ElapsedMilliseconds, request);
            }

            return response;
        }
    }
}
