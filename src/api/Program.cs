using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ugrc.api.Extensions;
using ugrc.api.Middleware;
using ugrc.api.Quirks;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureConfiguration();
    builder.ConfigureLogging();
    builder.ConfigureCors();
    builder.ConfigureVersioning();
    builder.ConfigureHealthChecks();
    builder.ConfigureDependencyInjection();
    builder.ConfigureHttpClients();
    builder.ConfigureOpenApi();

    var app = builder.Build();
    app.MapRoutes();
    app.UseMiddleware<RequestLoggerMiddleware>();
    app.UseMiddleware<JsonpMiddleware>();
    app.MapHealthChecks();
    app.MapOpenApi();
    logger.Information("Program configuration completed");
    app.Run();
} catch (Exception ex) {
    logger.Fatal(ex, "Host terminated unexpectedly");
} finally {
    logger.Information("Shutting down");
    Log.CloseAndFlush();
}
