using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Ai.Application.Commands;

public class DeleteAIPromptCommandHandler : CommandHandler, ICommandHandler<DeleteAIPromptCommand, AIPromptCommandResult>
{
    private readonly IValidator<DeleteAIPromptCommand> _validator;
    private readonly IAiRepository _promptRepository;

    public DeleteAIPromptCommandHandler(IValidator<DeleteAIPromptCommand> validator, IAiRepository promptRepository)
    {
        _validator = validator;
        _promptRepository = promptRepository;
    }

    public async Task<AIPromptCommandResult> HandleAsync(DeleteAIPromptCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid)
        {
            return new AIPromptCommandResult { ValidationResult = ValidationResult };
        }

        var prompt = await _promptRepository.GetById(command.Id);
        if (prompt is null)
        {
            AddError("Prompt não encontrado.");
            return new AIPromptCommandResult { ValidationResult = ValidationResult };
        }

        await _promptRepository.Remove(command.Id);

        return new AIPromptCommandResult { ValidationResult = ValidationResult, Prompt = AIPromptDto.FromEntity(prompt) };
    }
}
