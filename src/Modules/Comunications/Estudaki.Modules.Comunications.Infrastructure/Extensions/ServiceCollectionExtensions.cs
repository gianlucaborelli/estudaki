using Estudaki.Commons.Core.CQRS.Extensions;
using Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage;
using Estudaki.Modules.Comunications.Domain.Repositories;
using Estudaki.Modules.Comunications.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Estudaki.Modules.Comunications.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddComunicationsInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();


        services.AddValidatorsFromAssembly(typeof(CreateContactMessageCommandValidator).Assembly);

        services.AddCQRSHandlers(typeof(CreateContactMessageCommandHandler).Assembly);
    }
}
