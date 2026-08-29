using Estudaki.Commons.Core.CQRS;
using FluentValidation;

namespace Estudaki.Modules.Ai.Application.Commands;

public record CreateAIPromptCommand(string Name, string Content, string? Description) : ICommand<AIPromptCommandResult>;

public class CreateAIPromptCommandValidator : AbstractValidator<CreateAIPromptCommand>
{
    public CreateAIPromptCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome do prompt é obrigatório.")
            .MaximumLength(120)
            .WithMessage("O nome do prompt deve ter no máximo 120 caracteres.")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("O nome do prompt deve conter apenas letras minúsculas, números e hífens (ex.: \"review-question\").");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("O conteúdo do prompt é obrigatório.");
    }
}
