using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record AddExistingQuestionIntoExamCommand(QuestionDto Question, string ExamId) : ICommand<ValidationResult>;

public class AddExistingQuestionIntoExamCommandValidator : AbstractValidator<AddExistingQuestionIntoExamCommand>
{
    public AddExistingQuestionIntoExamCommandValidator()
    {
        RuleFor(x => x.Question).NotNull().WithMessage("Question cannot be null.");
        RuleFor(x => x.Question.QuestionId).NotEmpty().WithMessage("Question Id cannot be empty.");
        RuleFor(x => x.ExamId).NotEmpty().WithMessage("Exam Id cannot be empty.");
    }
}
