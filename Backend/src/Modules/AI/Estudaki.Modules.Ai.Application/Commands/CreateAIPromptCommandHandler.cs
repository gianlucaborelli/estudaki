using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;
using Estudaki.Modules.Ai.Domain.Entities;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public class CreateAIPromptCommandHandler : CommandHandler, ICommandHandler<CreateAIPromptCommand, CommandResult>
{
    private readonly IValidator<CreateAIPromptCommand> _validator;
    private readonly IAiRepository _promptRepository;

    public CreateAIPromptCommandHandler(IValidator<CreateAIPromptCommand> validator, IAiRepository promptRepository)
    {
        _validator = validator;
        _promptRepository = promptRepository;
    }

    public async Task<CommandResult> HandleAsync(CreateAIPromptCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var existing = await _promptRepository.GetByNameAsync(command.Name);
        if (existing is not null)
        {
            AddError($"Já existe um prompt cadastrado com o nome \"{command.Name}\".");
            return Result();
        }

        var prompt = new AIPrompt(command.Name, command.Content, command.Description);
        _promptRepository.Add(prompt);

        return Result(AIPromptDto.FromEntity(prompt));
    }
}
