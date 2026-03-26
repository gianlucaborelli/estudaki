using Estudaki.Infrastructure.Observability.Setups;
using Microsoft.AspNetCore.Builder;

namespace Estudaki.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static void AddObservability(this WebApplicationBuilder builder)
    {
        builder.LoggerInit();
        builder.MetricsInit();
        builder.TracingInit();
    }
}
