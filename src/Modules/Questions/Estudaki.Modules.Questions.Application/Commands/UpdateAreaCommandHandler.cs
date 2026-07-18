using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateAreaCommandHandler : CommandHandler, ICommandHandler<UpdateAreaCommand, ValidationResult>
{
    private readonly IValidator<UpdateAreaCommand> _validator;
    private readonly IAreaRepository _areaRepository;

    public UpdateAreaCommandHandler(IValidator<UpdateAreaCommand> validator, IAreaRepository areaRepository)
    {
        _validator = validator;
        _areaRepository = areaRepository;
    }

    public async Task<ValidationResult> HandleAsync(UpdateAreaCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid)
        {
            return ValidationResult;
        }

        var area = await _areaRepository.GetByIdAsync(command.Id);
        if (area == null)
        {
            AddError("Área não encontrada.");
            return ValidationResult;
        }

        area.Name = command.Name;
        area.Type = command.Type.ToString();

        await _areaRepository.UpdateAsync(area);

        return ValidationResult;
    }
}
