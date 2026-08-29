using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record CreatePublicNoticeCommand(PublicNoticeDto PublicNoticeDto) : ICommand<ValidationResult>;

public class CreatePublicNoticeCommandValidator : AbstractValidator<CreatePublicNoticeCommand>
{
    public CreatePublicNoticeCommandValidator()
    {
        RuleFor(x => x.PublicNoticeDto.Id).NotEmpty()
            .WithMessage("O ID do edital é obrigatório.");
    }
}
