using AGRC.api.Features.Converting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.Health;
using AGRC.api.Features.Milepost;
using AGRC.api.Features.Searching;
using AGRC.api.Middleware;
using AGRC.api.Models.ResponseContracts;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AGRC.api.Extensions;
public static class WebApplicationExtensions {
    public static void MapRoutes(this WebApplication app) {
        app.UseCorrelationId();

        if (app.Environment.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(1, 0)
            .HasApiVersion(2, 0)
            .ReportApiVersions()
            .Build();

        var api = app.MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);
        api.AddEndpointFilter<AuthorizeApiKeyFilter>();

        var geocode = api.MapGroup("/geocode");
        var search = api.MapGroup("/search");

        geocode.MapGet("/{street}/{zone}", async (
            [FromRoute] string street,
            [FromRoute] string zone,
            SingleGeocodeRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new GeocodeQuery.Query(street, zone, options));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<GeocodeQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .WithOpenApi(operation => new(operation) {
                OperationId = "Geocode",
                Summary = "Single address geocoding",
                Description = "Extract the x, y location from a street address and zone",
                Tags = new List<OpenApiTag> { new() { Name = "Address Geocoding" } },
            })
            .Produces<ApiResponseContract<SingleGeocodeResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/reverse/{x}/{y}", async (
            [FromRoute] double x,
            [FromRoute] double y,
            ReverseGeocodeRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var jsonOptions = factory.GetSerializerOptionsFor(apiVersion);
                return await mediator.Send(new ReverseGeocodeQuery.Query(x, y, options, jsonOptions));
            })
            .HasApiVersion(1)
            .HasApiVersion(2)
            .WithOpenApi(operation => new(operation) {
                OperationId = "ReverseGeocode",
                Summary = "Reverse Geocoding",
                Description = "Extract the x, y location from a street address and zone",
                Tags = new List<OpenApiTag> { new() { Name = "Address Geocoding" } },
            })
            .Produces<ApiResponseContract<ReverseGeocodeResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/milepost/{route}/{milepost}", async (
            [FromRoute] string route,
            [FromRoute] string milepost,
            RouteMilepostRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var jsonOptions = factory.GetSerializerOptionsFor(apiVersion);
                return await mediator.Send(new RouteMilepostQuery.Query(route, milepost, options, jsonOptions));
            })
            .HasApiVersion(1)
            .HasApiVersion(2)
            .WithOpenApi(operation => new(operation) {
                OperationId = "MilepostGeocode",
                Summary = "Milepost Geocoding",
                Description = "Extract the x, y location from a milepost along a route",
                Tags = new List<OpenApiTag> { new() { Name = "Milepost Geocoding" } },
            })
            .Produces<ApiResponseContract<RouteMilepostResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/reversemilepost/{x}/{y}", async (
            [FromRoute] double x,
            [FromRoute] double y,
            ReverseRouteMilepostRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var jsonOptions = factory.GetSerializerOptionsFor(apiVersion);
                return await mediator.Send(new ReverseRouteMilepostQuery.Query(x, y, options, jsonOptions));
            })
            .HasApiVersion(1)
            .HasApiVersion(2)
            .WithOpenApi(operation => new(operation) {
                OperationId = "ReverseMilepostGeocode",
                Summary = "Reverse Milepost Geocoding",
                Description = "Extract the x, y, and measure from a milepost (measure) along a route",
                Tags = new List<OpenApiTag> { new() { Name = "Milepost Geocoding" } },
            })
            .Produces<ApiResponseContract<RouteMilepostResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        search.MapGet("/{tableName}/{returnValues}", async (
            [FromRoute] string tableName,
            [FromRoute] string returnValues,
            SearchRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new SearchQuery.Query(tableName, returnValues, new SearchOptions(options)));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<SearchQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .WithOpenApi(operation => new(operation) {
                OperationId = "Search",
                Summary = "Search the OpenSGID",
                Description = "Search tables and attributes within the SGID",
                Tags = new List<OpenApiTag> { new() { Name = "Searching" } },
            })
            .Produces<ApiResponseContract<SearchResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);
    }
    public static void MapHealthChecks(this WebApplication app) {
        app.MapHealthChecks("/api/v1/health/details", new HealthCheckOptions {
            Predicate = healthCheck => healthCheck.Tags.Contains("health"),
            ResponseWriter = DetailedHealthCheckResponseWriter.WriteDetailsJson
        });

        app.MapHealthChecks("/api/v1/health", new HealthCheckOptions {
            Predicate = healthCheck => healthCheck.Tags.Contains("health"),
        });

        app.MapHealthChecks("", new HealthCheckOptions() {
            Predicate = healthCheck => healthCheck.Tags.Contains("startup"),
            ResponseWriter = StartupHealthCheckResponseWriter.WriteText
        });
    }
    public static void MapOpenApi(this WebApplication app) {
        app.UseSwagger(c => c.RouteTemplate = "openapi/{documentName}/api.json");
        app.UseSwaggerUI(c => {
            c.DocumentTitle = "UGRC API OpenAPI Documentation";
            c.RoutePrefix = "openapi";
            c.SwaggerEndpoint("/openapi/v1/api.json", "v1");
            c.SwaggerEndpoint("/openapi/v2/api.json", "v2");
            c.SupportedSubmitMethods();
            c.EnableDeepLinking();
            c.DocExpansion(DocExpansion.List);
        });
    }
}
