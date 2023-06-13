using AGRC.api.Extensions;
using AGRC.api.Quirks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("starting web host");

try {
    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureConfiguration();
    builder.ConfigureLogging();
    builder.ConfigureCors();
    builder.ConfigureJsonSerialization();
    builder.ConfigureVersioning();
    builder.ConfigureHealthChecks();
    builder.ConfigureDependencyInjection();
    builder.ConfigureHttpClients();
    builder.ConfigureOpenApi();

    var app = builder.Build();
    app.MapRoutes();
    app.UseMiddleware<JsonpMiddleware>();
    app.MapHealthChecks();
    app.MapOpenApi();
    logger.Information("program configuration completed");
    app.Run();
} catch (Exception ex) {
    logger.Fatal(ex, "host terminated unexpectedly");
} finally {
    logger.Information("shutting down");
    Log.CloseAndFlush();
}
