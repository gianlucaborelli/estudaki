using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UnifyQuestionCommand(List<string> QuestionsIds) : ICommand<ValidationResult>;

public class UnifyQuestionCommandValidator : AbstractValidator<UnifyQuestionCommand>
{
    public UnifyQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionsIds)
            .NotEmpty().WithMessage("Question IDs are required.");
    }
}
