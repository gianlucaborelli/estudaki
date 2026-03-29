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
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Message).MinimumLength(10);
        }
    }
}
