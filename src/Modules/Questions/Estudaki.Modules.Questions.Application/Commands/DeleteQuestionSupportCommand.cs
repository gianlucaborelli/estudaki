using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record DeleteQuestionSupportCommand(string QuestionSupportId) : ICommand<ValidationResult>;

public class DeleteQuestionSupportCommandValidator : AbstractValidator<DeleteQuestionSupportCommand>
{
    public DeleteQuestionSupportCommandValidator()
    {
        RuleFor(x => x.QuestionSupportId)
            .NotEmpty().WithMessage("Question support ID is required.");
    }
}
