using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudExtensions.Services;
using ProvaOnline.Components;
using ProvaOnline.Data;
using ProvaOnline.Data.Context;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddMudExtensions();
builder.Services.AddSingleton<IMongoContext, MongoContext>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddTransient<IQuestionServices, QuestionServices>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
