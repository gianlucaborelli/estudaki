using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UpdatePublicNoticeCommand(PublicNoticeDto PublicNoticeDto) : ICommand<ValidationResult>;

public class UpdatePublicNoticeCommandValidator : AbstractValidator<UpdatePublicNoticeCommand>
{
    public UpdatePublicNoticeCommandValidator()
    {
        RuleFor(x => x.PublicNoticeDto.Id).NotEmpty()
            .WithMessage("O ID do edital é obrigatório.");
    }
}