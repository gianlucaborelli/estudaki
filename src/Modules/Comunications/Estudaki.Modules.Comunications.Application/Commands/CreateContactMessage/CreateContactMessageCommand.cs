using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Comunications.Application.Commands.CreateContactMessage
{
    public record CreateContactMessageCommand
    (
        string Name, 
        string Email, 
        string Message, 
        bool CanBeReplied, 
        string? UserId
    ) : ICommand<ValidationResult>;

    public class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
    {
        public CreateContactMessageCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("O nome é obrigatório.");
            RuleFor(x => x.Email).EmailAddress()
                .WithMessage("O email deve ser válido.");
            RuleFor(x => x.Message).MinimumLength(10)
                .WithMessage("A mensagem deve ter pelo menos 10 caracteres.");
        }
    }
}
