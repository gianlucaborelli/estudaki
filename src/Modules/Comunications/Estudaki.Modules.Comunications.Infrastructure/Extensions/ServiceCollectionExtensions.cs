using Estudaki.Modules.Comunications.Domain.Repositories;
using Estudaki.Modules.Comunications.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Comunications.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
        public static void AddComunicationsInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IContactMessageRepository, ContactMessageRepository>();


    }
}
