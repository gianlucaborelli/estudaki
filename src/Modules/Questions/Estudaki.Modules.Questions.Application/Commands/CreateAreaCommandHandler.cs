using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class CreateAreaCommandHandler : CommandHandler, ICommandHandler<CreateAreaCommand, AreaCommandResult>
{
    private readonly IValidator<CreateAreaCommand> _validator;
    private readonly IAreaRepository _areaRepository;

    public CreateAreaCommandHandler(IValidator<CreateAreaCommand> validator, IAreaRepository areaRepository)
    {
        _validator = validator;
        _areaRepository = areaRepository;
    }

    public async Task<AreaCommandResult> HandleAsync(CreateAreaCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid)
        {
            return new AreaCommandResult { ValidationResult = ValidationResult };
        }

        var area = Area.Create(command.Name, command.Type);

        await _areaRepository.AddAsync(area);

        return new AreaCommandResult { ValidationResult = ValidationResult, Area = area.ToDto() };
    }
}
