using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.DTOs;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands.CreateNewPublicNotice;

public record CreateNewPublicNoticeCommand(PublicNoticeDto PublicNotice, ExamExtractionDto ExamExtraction) : ICommand<ValidationResult>;

public class CreateNewPublicNoticeCommandValidator : AbstractValidator<CreateNewPublicNoticeCommand>
{
    public CreateNewPublicNoticeCommandValidator()
    {
        RuleFor(x => x.PublicNotice).NotNull().WithMessage("PublicNotice cannot be null.");
        RuleFor(x => x.ExamExtraction).NotNull().WithMessage("ExamExtraction cannot be null.");
    }
}
