using EstudaKi.Web.Components;
using EstudaKi.Web.Data;
using EstudaKi.Web.Data.Context;
using EstudaKi.Web.Services;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

// MongoDB Context
builder.Services.AddSingleton<IMongoContext>(sp =>
    new MongoContext(sp.GetRequiredService<IConfiguration>()));

// Repositories
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IPublicNoticeRepository, PublicNoticeRepository>();

// Services
builder.Services.AddTransient<IQuestionServices, QuestionServices>();

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

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

