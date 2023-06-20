using System.IO;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Quirks;
public class JsonpMiddleware(RequestDelegate next) {
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context) {
        var version = context.GetRequestedApiVersion()!;

        if (version > ApiVersion.Default) {
            await _next(context);

            return;
        }

        if (!context.Request.Query.TryGetValue("callback", out var callback)) {
            await _next(context);

            return;
        }

        if (!context.Response.HasStarted) {
            context.Response.OnStarting(() => {
                context.Response.ContentType = "text/javascript; charset=utf-8";

                return Task.CompletedTask;
            });
        }

        var originalResponseStream = context.Response.Body;
        using var stream = new MemoryStream();
        context.Response.Body = stream;

        await _next(context);

        stream.Position = 0;
        context.Response.Body = originalResponseStream;

        await context.Response.WriteAsync($"/**/ typeof {callback} === 'function' && {callback}(");
        await stream.CopyToAsync(context.Response.Body);
        await context.Response.WriteAsync(");");
    }
}
