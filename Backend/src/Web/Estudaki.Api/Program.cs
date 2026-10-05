using Estudaki.Infrastructure.Crosscutting;
using Estudaki.Modules.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Otimização de autenticação: remove cookies para endpoints públicos
// Deve estar ANTES de UseAuthentication para interceptar a requisição
app.UseOptimizedAuthentication();

// The API is only reached over plain HTTP, on the container's internal network
// (frontend calls it via loopback, and TLS termination happens at Traefik).
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
