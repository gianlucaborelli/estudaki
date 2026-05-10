using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record DeleteQuestionCommand(string QuestionId, string ExamId) : ICommand<ValidationResult>;

public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
{
    public DeleteQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotNull().WithMessage("QuestionId não pode ser nulo.");
        RuleFor(x => x.ExamId).NotNull().WithMessage("ExamId não pode ser nulo.");
    }
}
