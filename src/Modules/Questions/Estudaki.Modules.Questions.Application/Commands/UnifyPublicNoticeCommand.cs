using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UnifyPublicNoticeCommand (List<string> PublicNoticeIds) : ICommand<ValidationResult>;

public class UnifyPublicNoticeCommandValidator : AbstractValidator<UnifyPublicNoticeCommand>
{
    public UnifyPublicNoticeCommandValidator()
    {
        RuleFor(x => x.PublicNoticeIds)
            .NotEmpty().WithMessage("Public notice IDs are required.");            
    }
}