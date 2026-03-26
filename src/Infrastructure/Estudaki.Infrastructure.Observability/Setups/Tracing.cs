using Estudaki.Infrastructure.Observability.Configs;
using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Trace;

namespace Estudaki.Infrastructure.Observability.Setups;

internal static class Tracing
{
    internal static void TracingInit(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing((traceBuilder) =>
            {
                traceBuilder
                    .AddSource(OpenTelemetryExtensions.ActivitySource.Name)
                    .SetSampler(new AlwaysOnSampler())
                    .UseGrafana()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddProcessor(new CustomTraceProcessor())   
                    .AddOtlpExporter();
            });
    }
}
