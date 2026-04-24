using Estudaki.Commons.Core.Storage;
using Estudaki.Infrastructure.Crosscutting;
using Estudaki.Infrastructure.Observability;
using EstudaKi.Web.Components;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;
       
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();
builder.AddObservability();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var app = builder.Build();

app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
    var start = DateTime.UtcNow;

    var request = context.Request;
    var ip = context.Connection.RemoteIpAddress?.ToString();
    var path = request.Path.Value ?? "";
    var method = request.Method;
    var userAgent = request.Headers["User-Agent"].ToString();

    var sessionId = context.Request.Cookies["sid"];

    if (string.IsNullOrEmpty(sessionId))
    {
        sessionId = Guid.NewGuid().ToString();

        context.Response.Cookies.Append("sid", sessionId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax
        });
    }

    await next();

    var statusCode = context.Response.StatusCode;
    var duration = DateTime.UtcNow - start;

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    var pathLower = path.ToLowerInvariant();

    var isSuspicious =
        pathLower.StartsWith("/wp-") ||
        pathLower.Contains("php") ||
        pathLower.Contains(".env") ||
        pathLower.Contains("admin") ||
        pathLower.Contains("boaform");

    var logLevel =
        statusCode >= 500 ? LogLevel.Error :
        isSuspicious ? LogLevel.Warning :
        statusCode == 404 ? LogLevel.Information :
        LogLevel.Information;

    logger.Log(logLevel,
        "HTTP {@Data}",
        new
        {
            Method = method,
            Path = path,
            StatusCode = statusCode,
            DurationMs = duration.TotalMilliseconds,
            IP = ip,
            UserAgent = userAgent,
            SessionId = sessionId,
            IsSuspicious = isSuspicious
        });
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseObservability();
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

