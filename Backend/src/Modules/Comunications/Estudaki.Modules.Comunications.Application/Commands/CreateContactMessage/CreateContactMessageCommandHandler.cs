using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Comunications.Domain.Entities;
using Estudaki.Modules.Comunications.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage;

public class CreateContactMessageCommandHandler : CommandHandler, ICommandHandler<CreateContactMessageCommand, CommandResult>
{
    private readonly IContactMessageRepository _contactMessageRepository;
    private readonly IValidator<CreateContactMessageCommand> _validator;

    public CreateContactMessageCommandHandler(IContactMessageRepository contactMessageRepository, IValidator<CreateContactMessageCommand> validator) : base()
    {
        _contactMessageRepository = contactMessageRepository;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(CreateContactMessageCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var contactMessage = new ContactMessage(
            command.Name,
            command.Email,
            command.Message,
            command.CanBeReplied,
            command.UserId
        );

        _contactMessageRepository.Add( contactMessage );

        return Result();
    }
}
