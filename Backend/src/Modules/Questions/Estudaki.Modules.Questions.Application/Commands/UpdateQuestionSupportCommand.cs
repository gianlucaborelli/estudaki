using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UpdateQuestionSupportCommand(QuestionSupportDto QuestionSupportDto) : ICommand<ValidationResult>;

public class UpdateQuestionSupportCommandValidator : AbstractValidator<UpdateQuestionSupportCommand>
{
    public UpdateQuestionSupportCommandValidator()
    {
        RuleFor(x => x.QuestionSupportDto.Id).NotEmpty()
            .WithMessage("O ID do suporte de questão é obrigatório.");
    }
}