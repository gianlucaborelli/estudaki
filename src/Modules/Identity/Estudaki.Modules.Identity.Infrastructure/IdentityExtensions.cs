using Estudaki.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazorWebApp1.Data;

namespace Estudaki.Modules.Identity.Infrastructure;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
            .AddIdentityCookies();
        var connectionString = configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }

}
