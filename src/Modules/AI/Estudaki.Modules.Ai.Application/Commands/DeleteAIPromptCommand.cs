using Estudaki.Commons.Core.CQRS;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public record DeleteAIPromptCommand(string Id) : ICommand<AIPromptCommandResult>;

public class DeleteAIPromptCommandValidator : AbstractValidator<DeleteAIPromptCommand>
{
    public DeleteAIPromptCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do prompt é obrigatório.");
    }
}
