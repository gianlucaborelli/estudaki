using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Identity.Adapter.Services;
using Estudaki.Modules.Identity.Application.Commands;
using Estudaki.Modules.Identity.Application.Commands.Login;
using Estudaki.Modules.Identity.Application.Commands.Register;
using Estudaki.Modules.Identity.Domain.Entities;
using Estudaki.Modules.Identity.Domain.Interfaces;
using Estudaki.Modules.Identity.Infrastructure.Data;
using Estudaki.Modules.Identity.Infrastructure.Data.Seeds;
using Estudaki.Modules.Identity.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Identity.Infrastructure;

public static class IdentityExtensions
{
    /// <summary>
    /// Registra os serviços de identidade do módulo, incluindo autenticação com cookies
    /// e otimizações de performance para endpoints públicos.
    /// </summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        }).AddIdentityCookies();

        var connectionString = configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromMinutes(30);
        });

        services.AddHostedService<IdentitySeedHostedService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        // Registrar Command Handlers
        services.AddScoped<ICommandHandler<LoginCommand, CommandResult>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<RegisterUserCommand, CommandResult>, RegisterUserCommandHandler>();

        return services;
    }

    /// <summary>
    /// Registra o middleware que otimiza a validação de autenticação para endpoints públicos.
    /// Deve ser chamado ANTES de app.UseAuthentication() no pipeline.
    /// </summary>
    public static IApplicationBuilder UseOptimizedAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SkipAuthenticationCookieMiddleware>();
    }
}
