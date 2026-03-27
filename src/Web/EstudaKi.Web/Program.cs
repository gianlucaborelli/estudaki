using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Infrastructure.Observability;
using Estudaki.Modules.Questions.Infrastructure.Extensions;
using EstudaKi.Web.Components;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddMudExtensions();
builder.AddObservability();

// Add CQRS
builder.Services.AddCQRS();

// Add Questions Module
var mongoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("MongoDB connection string not found");
builder.Services.AddQuestionsModule(mongoConnectionString, "ProvaOnlineV2");

builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers();

var app = builder.Build();

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

