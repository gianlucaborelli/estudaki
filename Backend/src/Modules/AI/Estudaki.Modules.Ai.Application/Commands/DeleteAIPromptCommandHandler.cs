using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Ai.Application.Commands;

public class DeleteAIPromptCommandHandler : CommandHandler, ICommandHandler<DeleteAIPromptCommand, CommandResult>
{
    private readonly IValidator<DeleteAIPromptCommand> _validator;
    private readonly IAiRepository _promptRepository;

    public DeleteAIPromptCommandHandler(IValidator<DeleteAIPromptCommand> validator, IAiRepository promptRepository)
    {
        _validator = validator;
        _promptRepository = promptRepository;
    }

    public async Task<CommandResult> HandleAsync(DeleteAIPromptCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();


        var prompt = await _promptRepository.GetById(command.Id);
        if (prompt is null)
        {
            AddError("Prompt não encontrado.");
            return Result();
        }

        await _promptRepository.Remove(command.Id);

        return Result(AIPromptDto.FromEntity(prompt));
    }
}
