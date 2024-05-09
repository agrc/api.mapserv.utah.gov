namespace ugrc.api.Middleware;

public interface IKeyProvider {
    string? Get(HttpRequest request);
}
public interface IBrowserKeyProvider : IKeyProvider {
}
public interface IServerIpProvider : IKeyProvider {
}

public class LocalServerIpProvider : IServerIpProvider {
    public string Get(HttpRequest request) => request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
}
public class FirebaseClientIpProvider : IServerIpProvider {
    public string Get(HttpRequest request) {
        var value = request.Headers["X-Forwarded-For"].FirstOrDefault() ?? string.Empty;

        return value.Contains(',') ? value.Split(',').First() : value;
    }
}

public class BrowserKeyProvider : IBrowserKeyProvider {
    public string? Get(HttpRequest request) {
        if (request.Query.TryGetValue("apikey", out var queryStringKey)) {
            return queryStringKey.ToString().ToLowerInvariant();
        }

        try {
            var formData = request.Form;

            if (formData.TryGetValue("apikey", out var formKey)) {
                return formKey.ToString().ToLowerInvariant();
            }
        } catch (InvalidOperationException) {
            // ignored because it's not a form content type
        }

        return null;
    }
}
