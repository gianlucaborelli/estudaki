using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands;

public record CreateQuestionCommand(QuestionDto Question) : ICommand<CommandResult>;

public class AddNewQuestionIntoExamCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public AddNewQuestionIntoExamCommandValidator()
    {
        RuleFor(x => x.Question).NotNull().WithMessage("Question cannot be null.");
        RuleFor(x => x.Question.PublicNoticeId).NotEmpty().WithMessage("Public Notice Id cannot be empty.");
        RuleFor(x => x.Question.ExamId).NotEmpty().WithMessage("Exam Id cannot be empty.");
    }
}
