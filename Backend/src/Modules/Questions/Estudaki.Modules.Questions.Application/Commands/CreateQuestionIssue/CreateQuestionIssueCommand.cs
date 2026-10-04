using System;
using System.Collections.Generic;
using System.Text;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands.CreateQuestionIssue;

public record CreateQuestionIssueCommand : ICommand<CommandResult>
{    
    public string QuestionId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string UserEmail { get; set; } = default!;
    public bool CanBeReplied { get; set; } = false;
    public string Type { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateQuestionIssueCommandValidator : AbstractValidator<CreateQuestionIssueCommand>
{
    public CreateQuestionIssueCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty()
            .WithMessage("O ID da questão é obrigatório.");
        RuleFor(x => x.UserName).NotEmpty()
            .WithMessage("O nome do usuário é obrigatório.");
        RuleFor(x => x.UserEmail).NotEmpty()
            .WithMessage("O email do usuário é obrigatório.");
        RuleFor(x => x.Type).NotEmpty()
            .WithMessage("O tipo da questão é obrigatório.");
        RuleFor(x => x.Description).NotEmpty()
            .WithMessage("A descrição da questão é obrigatória.");
    }
}
