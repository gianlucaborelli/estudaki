using OpenTelemetry;
using System.Diagnostics;

namespace Estudaki.Infrastructure.Observability.Configs;

internal class CustomTraceProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        base.OnStart(activity);
    }

    public override void OnEnd(Activity activity)
    {
        if (activity.Tags.Any(tag => tag.Key == "url.path" && tag.Value.Contains("/metrics")))
        {
            activity.ActivityTraceFlags = ActivityTraceFlags.None;
            return;
        }

        base.OnEnd(activity);
    }
}
