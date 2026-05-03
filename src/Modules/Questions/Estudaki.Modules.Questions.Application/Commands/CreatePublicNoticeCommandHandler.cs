using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class CreatePublicNoticeCommandHandler : CommandHandler, ICommandHandler<CreatePublicNoticeCommand, ValidationResult>
{
    private readonly IValidator<CreatePublicNoticeCommand> _validator;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public CreatePublicNoticeCommandHandler(IValidator<CreatePublicNoticeCommand> validator,
        IPublicNoticeRepository publicNoticeRepository)
    {
        _validator = validator;
        _publicNoticeRepository = publicNoticeRepository;
    }

    public async Task<ValidationResult> HandleAsync(CreatePublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result;
        }

        var publicNotice = command.PublicNoticeDto.ToEntity();

        _publicNoticeRepository.Add(publicNotice);

        return ValidationResult;
    }
}
