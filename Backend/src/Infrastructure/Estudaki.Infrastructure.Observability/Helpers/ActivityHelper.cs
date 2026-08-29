using Estudaki.Infrastructure.Observability.Configs;
using System.Diagnostics;

namespace Estudaki.Infrastructure.Observability.Helpers;

public static class ActivityHelper
{
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return OpenTelemetryExtensions.ActivitySource.StartActivity(name, kind);
    }

    public static void EnrichActivity(string key, object? value)
    {
        Activity.Current?.SetTag(key, value);
    }

    public static void RecordException(Exception exception)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.RecordException(exception);
        }
    }

    public static void AddEvent(string name, Dictionary<string, object?>? tags = null)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            if (tags != null)
            {
                var activityTags = new ActivityTagsCollection(tags);
                var activityEvent = new ActivityEvent(name, tags: activityTags);
                activity.AddEvent(activityEvent);
            }
            else
            {
                var activityEvent = new ActivityEvent(name);
                activity.AddEvent(activityEvent);
            }
        }
    }
}

public static class ActivityExtensions
{
    public static void RecordException(this Activity activity, Exception exception)
    {
        var tags = new ActivityTagsCollection
        {
            { "exception.type", exception.GetType().FullName },
            { "exception.message", exception.Message },
            { "exception.stacktrace", exception.StackTrace }
        };

        activity.AddEvent(new ActivityEvent("exception", tags: tags));
    }
}
