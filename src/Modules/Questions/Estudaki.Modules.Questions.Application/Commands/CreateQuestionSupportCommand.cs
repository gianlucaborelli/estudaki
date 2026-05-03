using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record CreateQuestionSupportCommand(QuestionSupportDto QuestionSupportDto, string PublicNoticeId) : ICommand<ValidationResult>;

public class CreateQuestionSupportCommandValidator : AbstractValidator<CreateQuestionSupportCommand>
{
    public CreateQuestionSupportCommandValidator()
    {
        RuleFor(x => x.QuestionSupportDto).NotNull()
            .WithMessage("O suporte de questão é obrigatório.");
        RuleFor(x => x.PublicNoticeId).NotEmpty()
            .WithMessage("O ID do edital é obrigatório.");
    }
}