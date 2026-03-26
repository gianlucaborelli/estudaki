using Estudaki.Infrastructure.Observability.Configs;
using Grafana.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Estudaki.Infrastructure.Observability.Setups;

internal static class Metrics
{
    internal static IHostApplicationBuilder MetricsInit(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: OpenTelemetryExtensions.ServiceName,
                    serviceVersion: OpenTelemetryExtensions.ServiceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName,
                    ["host.name"] = Environment.MachineName
                }))
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder
                    .AddMeter(OpenTelemetryExtensions.ServiceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddNpgsqlInstrumentation()
                    .UseGrafana()
                    .AddOtlpExporter();
            });

        return builder;
    }
}
