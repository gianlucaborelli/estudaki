using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class CreatePublicNoticeCommandHandler : CommandHandler, ICommandHandler<CreatePublicNoticeCommand, CommandResult>
{
    private readonly IValidator<CreatePublicNoticeCommand> _validator;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public CreatePublicNoticeCommandHandler(IValidator<CreatePublicNoticeCommand> validator,
        IPublicNoticeRepository publicNoticeRepository)
    {
        _validator = validator;
        _publicNoticeRepository = publicNoticeRepository;
    }

    public async Task<CommandResult> HandleAsync(CreatePublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success)
        {
            return Result();
        }

        var publicNotice = command.PublicNoticeDto.ToEntity();

        _publicNoticeRepository.Add(publicNotice);

        return Result();
    }
}
