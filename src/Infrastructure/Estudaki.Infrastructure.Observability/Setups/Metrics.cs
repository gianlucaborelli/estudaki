using Grafana.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;

namespace Estudaki.Infrastructure.Observability.Setups;

internal static class Metrics
{
    internal static IHostApplicationBuilder MetricsInit(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder
                .AddAspNetCoreInstrumentation()
                .AddProcessInstrumentation()
                .AddSqlClientInstrumentation()
                .AddNpgsqlInstrumentation()
                    .UseGrafana();
            });


        return builder;
    }
}
