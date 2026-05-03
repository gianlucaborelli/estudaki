using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record DeleteQuestionCommand(string QuestionId) : ICommand<ValidationResult>;

public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotNull().WithMessage("QuestionId não pode ser nulo.");
    }
}
