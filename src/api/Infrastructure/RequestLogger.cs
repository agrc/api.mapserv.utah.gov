using MediatR.Pipeline;

namespace ugrc.api.Infrastructure;
public class RequestLogger<TRequest>(ILogger log) : IRequestPreProcessor<TRequest>
    where TRequest : notnull {
    private readonly ILogger? _log = log?.ForContext<IMediator>();

    public Task Process(TRequest request, CancellationToken cancellationToken) {
        var name = typeof(TRequest).FullName;

        _log?.ForContext("request", request)
            .Debug("Mediatr processing: {name}", name);

        return Task.CompletedTask;
    }
}
