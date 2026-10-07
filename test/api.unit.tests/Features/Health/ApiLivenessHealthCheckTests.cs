using Google.Api.Gax.Grpc;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ugrc.api.Extensions;
using ugrc.api.Features.Health;
using ugrc.api.Services;
using ZiggyCreatures.Caching.Fusion;

namespace api.tests.Features.Health;

public class ApiLivenessHealthCheckTests {
    [Fact]
    public async Task Should_remain_unhealthy_after_instance_failure() {
        var check = new ApiLivenessHealthCheck();
        var context = new HealthCheckContext();

        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Healthy);

        check.MarkUnhealthy();

        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Unhealthy);
        (await check.CheckHealthAsync(context)).Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task Should_report_unhealthy_instances_to_cloud_run() {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.ConfigureHealthChecks();
        builder.Services.AddSingleton<ApiLivenessHealthCheck>();

        await using var app = builder.Build();
        app.MapHealthChecks();
        await app.StartAsync();

        using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
        (await client.GetAsync("/api/v1/health/live")).StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        app.Services.GetRequiredService<ApiLivenessHealthCheck>().MarkUnhealthy();

        (await client.GetAsync("/api/v1/health/live")).StatusCode.ShouldBe(System.Net.HttpStatusCode.ServiceUnavailable);
    }

    [Theory]
    [InlineData("Google.Cloud.Firestore.V1.Firestore", HealthStatus.Unhealthy)]
    [InlineData("Other.Type", HealthStatus.Healthy)]
    public async Task Should_only_mark_failed_firestore_initializer_unhealthy(string typeName, HealthStatus expectedStatus) {
        var firestoreClient = new Mock<FirestoreClient>(MockBehavior.Strict);
        firestoreClient.SetupGet(client => client.Settings).Returns(new FirestoreSettings());
        firestoreClient.Setup(client => client.BatchGetDocuments(It.IsAny<BatchGetDocumentsRequest>(), It.IsAny<CallSettings>()))
            .Throws(new TypeInitializationException(typeName, new OutOfMemoryException()));

        var services = new ServiceCollection();
        services.AddFusionCache("firestore");
        using var provider = services.BuildServiceProvider();
        var liveness = new ApiLivenessHealthCheck();
        var repository = new FirestoreApiKeyRepository(FirestoreDb.Create("test-project", firestoreClient.Object),
            provider.GetRequiredService<IFusionCacheProvider>(), liveness);

        await Should.ThrowAsync<TypeInitializationException>(() => repository.GetKey("test-key"));
        (await liveness.CheckHealthAsync(new HealthCheckContext())).Status.ShouldBe(expectedStatus);
    }
}
