using Estudaki.Infrastructure.Observability.Helpers;
using Estudaki.Infrastructure.Observability.Middlewares;
using Estudaki.Infrastructure.Observability.Setups;
using EstudaKi.Infrastructure.Observability.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static void AddObservability(this WebApplicationBuilder builder)
    {
        builder.LoggerInit();
        builder.MetricsInit();
        builder.TracingInit();

        builder.Services.AddScoped<NavigationTracker>();
        builder.Services.AddHttpContextAccessor();
    }

    public static void UseObservability(this WebApplication app)
    {
        //app.UseErrorLogging();
        //app.UseMiddleware<ObservabilityMiddleware>();
    }
}
