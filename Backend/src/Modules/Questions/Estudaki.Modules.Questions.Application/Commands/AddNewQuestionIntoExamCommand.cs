using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record AddNewQuestionIntoExamCommand(QuestionDto Question) : ICommand<ValidationResult>;

public class AddNewQuestionIntoExamCommandValidator : AbstractValidator<AddNewQuestionIntoExamCommand>
{
    public AddNewQuestionIntoExamCommandValidator()
    {
        RuleFor(x => x.Question).NotNull().WithMessage("Question cannot be null.");
        RuleFor(x => x.Question.PublicNoticeId).NotEmpty().WithMessage("Public Notice Id cannot be empty.");
        RuleFor(x => x.Question.ExamId).NotEmpty().WithMessage("Exam Id cannot be empty.");
    }
}
