using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public record UpdateExamCommand(Exam Exam) : ICommand<ValidationResult>;

public class UpdateExamCommandValidator : AbstractValidator<UpdateExamCommand>
{
    public UpdateExamCommandValidator()
    {
        RuleFor(x => x.Exam.Id).NotEmpty()
            .WithMessage("O ID do exame é obrigatório.");
    }
}