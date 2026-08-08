using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public class UpdateAIPromptCommandHandler : CommandHandler, ICommandHandler<UpdateAIPromptCommand, AIPromptCommandResult>
{
    private readonly IValidator<UpdateAIPromptCommand> _validator;
    private readonly IAiRepository _promptRepository;

    public UpdateAIPromptCommandHandler(IValidator<UpdateAIPromptCommand> validator, IAiRepository promptRepository)
    {
        _validator = validator;
        _promptRepository = promptRepository;
    }

    public async Task<AIPromptCommandResult> HandleAsync(UpdateAIPromptCommand command, CancellationToken cancellationToken = default)
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

        prompt.UpdateContent(command.Content, command.Description);
        await _promptRepository.Update(prompt);

        return new AIPromptCommandResult { ValidationResult = ValidationResult, Prompt = AIPromptDto.FromEntity(prompt) };
    }
}
