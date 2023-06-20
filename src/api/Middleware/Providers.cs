namespace AGRC.api.Middleware;

public interface IKeyProvider {
    string? Get(HttpRequest request);
}
public interface IBrowserKeyProvider : IKeyProvider {
}
public interface IServerIpProvider : IKeyProvider {
}

public class ServerIpProvider : IServerIpProvider {
    public string Get(HttpRequest request) => request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
}

public class BrowserKeyProvider : IBrowserKeyProvider {
    public string? Get(HttpRequest request) {
        if (request.Query.TryGetValue("apikey", out var queryStringKey)) {
            return queryStringKey.ToString();
        }

        try {
            var formData = request.Form;

            if (formData.TryGetValue("apikey", out var formKey)) {
                return formKey.ToString();
            }
        } catch (InvalidOperationException) {
            // ignored because it's not a form content type
        }

        return null;
    }
}
