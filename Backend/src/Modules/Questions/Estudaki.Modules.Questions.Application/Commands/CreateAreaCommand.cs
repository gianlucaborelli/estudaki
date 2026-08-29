using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record CreateAreaCommand(string Name, AreaType Type) : ICommand<AreaCommandResult>;

public class CreateAreaCommandValidator : AbstractValidator<CreateAreaCommand>
{
    public CreateAreaCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome da área é obrigatório.")
            .MaximumLength(120)
            .WithMessage("O nome da área deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("O tipo da área é inválido.");
    }
}
