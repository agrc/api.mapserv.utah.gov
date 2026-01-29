using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ugrc.api.Features.Converting;
using ugrc.api.Features.Geocoding;
using ugrc.api.Features.Health;
using ugrc.api.Features.Information;
using ugrc.api.Features.Milepost;
using ugrc.api.Features.Searching;
using ugrc.api.Middleware;
using ugrc.api.Models.ResponseContracts;
using Microsoft.OpenApi;

namespace ugrc.api.Extensions;

public static class WebApplicationExtensions {
    public static void MapRoutes(this WebApplication app) {
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
        var info = api.MapGroup("/info");

        geocode.MapGet("/{street}/{zone}", async (
            [FromRoute] string street,
            [FromRoute] string zone,
            SingleGeocodeRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new GeocodeQuery.Query(street, zone, options, apiVersion));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<GeocodeQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "Geocode";
                operation.Summary = "Single address geocoding";
                operation.Description = "Extract the x, y location from a street address and zone";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Address Geocoding")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<SingleGeocodeResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/reverse/{x:double}/{y:double}", async (
            [FromRoute] double x,
            [FromRoute] double y,
            ReverseGeocodeRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new ReverseGeocodeQuery.Query(x, y, options));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "ReverseGeocode";
                operation.Summary = "Reverse Geocoding";
                operation.Description = "Extract the x, y location from a street address and zone";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Address Geocoding")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<ReverseGeocodeResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/milepost/{route}/{milepost:double}", async (
            [FromRoute] string route,
            [FromRoute] string milepost,
            RouteMilepostRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new RouteMilepostQuery.Query(route, milepost, options, apiVersion));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<RouteMilepostQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "MilepostGeocode";
                operation.Summary = "Milepost Geocoding";
                operation.Description = "Extract the x, y location from a milepost along a route";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Milepost Geocoding")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<RouteMilepostResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        geocode.MapGet("/reversemilepost/{x:double}/{y:double}", async (
            [FromRoute] double x,
            [FromRoute] double y,
            ReverseRouteMilepostRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new ReverseRouteMilepostQuery.Query(x, y, options));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "ReverseMilepostGeocode";
                operation.Summary = "Reverse Milepost Geocoding";
                operation.Description = "Extract the x, y, and measure from a milepost (measure) along a route";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Milepost Geocoding")
                };

                return Task.CompletedTask;
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
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "Search";
                operation.Summary = "Search the Open SGID";
                operation.Description = "Search tables and attributes within the SGID";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Searching")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<SearchResponseContract>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        _ = info.MapGet("/featureclassnames", async (
            InformationRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new SqlSchemaQuery.Query(options.SgidCategory!));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<SqlSchemaQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "FeatureClassNames";
                operation.Summary = "Get all feature classes for a SGID category";
                operation.Description = "Discover SGID data by viewing all available feature classes for a given category";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Info")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<IReadOnlyCollection<string>>>(StatusCodes.Status200OK)
            .Produces<ApiResponseContract>(StatusCodes.Status400BadRequest)
            .Produces<ApiResponseContract>(StatusCodes.Status404NotFound)
            .Produces<ApiResponseContract>(StatusCodes.Status500InternalServerError);

        info.MapGet("/fieldnames/{tableName}", async (
            [FromRoute] string tableName,
            InformationRequestOptionsContract options,
            [FromServices] IMediator mediator,
            [FromServices] IJsonSerializerOptionsFactory factory,
            [FromServices] ApiVersion apiVersion)
            => {
                var result = await mediator.Send(new SqlAttributeQuery.Query(tableName.ToLowerInvariant().Trim(), options.SgidCategory!));

                return TypedResults.Json(result, factory.GetSerializerOptionsFor(apiVersion), "application/json", result.Status);
            })
            .AddEndpointFilter<SqlAttributeQuery.ValidationFilter>()
            .HasApiVersion(1)
            .HasApiVersion(2)
            .AddOpenApiOperationTransformer((operation, context, ct) => {
                operation.OperationId = "FeatureClassAttributeNames";
                operation.Summary = "Get all attributes for a SGID feature class";
                operation.Description = "Understand SGID table available fields by viewing all of the searchable attributes";
                operation.Tags = new HashSet<OpenApiTagReference> {
                    new("Info")
                };

                return Task.CompletedTask;
            })
            .Produces<ApiResponseContract<IReadOnlyCollection<string>>>(StatusCodes.Status200OK)
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
    public static void MapOpenApi(this WebApplication app)
        => app.MapOpenApi("/openapi/{documentName}/api.json");
}
