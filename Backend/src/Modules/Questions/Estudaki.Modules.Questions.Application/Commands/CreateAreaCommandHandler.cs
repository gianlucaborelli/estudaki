using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class CreateAreaCommandHandler : CommandHandler, ICommandHandler<CreateAreaCommand, CommandResult>
{
    private readonly IValidator<CreateAreaCommand> _validator;
    private readonly IAreaRepository _areaRepository;

    public CreateAreaCommandHandler(IValidator<CreateAreaCommand> validator, IAreaRepository areaRepository)
    {
        _validator = validator;
        _areaRepository = areaRepository;
    }

    public async Task<CommandResult> HandleAsync(CreateAreaCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success)
        {
            return Result();
        }

        var area = Area.Create(command.Name, command.Type);

        await _areaRepository.AddAsync(area);

        return Result(area.ToDto());
    }
}
