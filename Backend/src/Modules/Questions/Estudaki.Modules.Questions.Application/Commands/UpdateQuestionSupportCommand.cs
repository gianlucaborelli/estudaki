using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UpdateQuestionSupportCommand(QuestionSupportDto QuestionSupport) : ICommand<CommandResult>;

public class UpdateQuestionSupportCommandValidator : AbstractValidator<UpdateQuestionSupportCommand>
{
    public UpdateQuestionSupportCommandValidator()
    {
        RuleFor(x => x.QuestionSupport.Id).NotEmpty()
            .WithMessage("O ID do suporte de questão é obrigatório.");
    }
}