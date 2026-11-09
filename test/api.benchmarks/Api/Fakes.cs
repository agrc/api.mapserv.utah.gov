using System.Collections.Concurrent;
using System.Reflection;
using StackExchange.Redis;
using ugrc.api.Infrastructure;
using ugrc.api.Middleware;
using ugrc.api.Models;
using ugrc.api.Models.Linkables;
using ugrc.api.Services;

namespace benchmark.api;

// hand written fakes keep mocking framework overhead out of the measurements

public class FakeComputeMediator(IReadOnlyCollection<GridLinkable> grids) : IComputeMediator {
    private readonly IReadOnlyCollection<GridLinkable> _grids = grids;

    public Task<TResult> Handle<TResult>(IComputation<TResult> command, CancellationToken cancellationToken) =>
        Task.FromResult((TResult)_grids);
}

public class FakeApiKeyRepository(ApiKey key) : IApiKeyRepository {
    private readonly Task<ApiKey> _key = Task.FromResult(key);

    public Task<ApiKey> GetKey(string key) => _key;
}

public class QueryStringKeyProvider : IBrowserKeyProvider {
    public string? Get(HttpRequest request) =>
        request.Query.TryGetValue("apikey", out var key) ? key.ToString().ToLowerInvariant() : null;
}

public class ForwardedForIpProvider : IServerIpProvider {
    public string? Get(HttpRequest request) => request.Headers["X-Forwarded-For"].FirstOrDefault();
}

/// <summary>
/// Implements any interface by returning completed tasks or defaults. Used for the redis connection and database
/// since those interfaces have hundreds of members and the api only fires and forgets against them.
/// </summary>
public class NullProxy : DispatchProxy {
    private static readonly ConcurrentDictionary<Type, object?> _results = new();

    public static T Create<T>() where T : class => Create<T, NullProxy>();

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) {
        var type = targetMethod?.ReturnType;

        if (type is null || type == typeof(void)) {
            return null;
        }

        return _results.GetOrAdd(type, CreateResult);
    }

    private static object? CreateResult(Type type) {
        if (type == typeof(IDatabase)) {
            return Create<IDatabase>();
        }

        if (type == typeof(Task)) {
            return Task.CompletedTask;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)) {
            var resultType = type.GetGenericArguments()[0];
            var value = resultType.IsValueType ? Activator.CreateInstance(resultType) : null;

            return typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, [value]);
        }

        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
