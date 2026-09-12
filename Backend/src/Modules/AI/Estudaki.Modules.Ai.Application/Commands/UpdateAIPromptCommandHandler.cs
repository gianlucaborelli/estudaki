using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public class UpdateAIPromptCommandHandler : CommandHandler, ICommandHandler<UpdateAIPromptCommand, CommandResult>
{
    private readonly IValidator<UpdateAIPromptCommand> _validator;
    private readonly IAiRepository _promptRepository;

    public UpdateAIPromptCommandHandler(IValidator<UpdateAIPromptCommand> validator, IAiRepository promptRepository)
    {
        _validator = validator;
        _promptRepository = promptRepository;
    }

    public async Task<CommandResult> HandleAsync(UpdateAIPromptCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var prompt = await _promptRepository.GetById(command.Id);
        if (prompt is null)
        {
            AddError("Prompt não encontrado.");
            return Result();
        }

        prompt.UpdateContent(command.Content, command.Description);
        await _promptRepository.Update(prompt);

        return Result(AIPromptDto.FromEntity(prompt));
    }
}
