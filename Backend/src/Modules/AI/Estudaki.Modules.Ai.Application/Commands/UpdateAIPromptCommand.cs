using Estudaki.Commons.Core.CQRS;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public record UpdateAIPromptCommand(string Id, string Content, string? Description) : ICommand<AIPromptCommandResult>;

public class UpdateAIPromptCommandValidator : AbstractValidator<UpdateAIPromptCommand>
{
    public UpdateAIPromptCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do prompt é obrigatório.");
        RuleFor(x => x.Content).NotEmpty().WithMessage("O conteúdo do prompt é obrigatório.");
    }
}
