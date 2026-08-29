using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionSupportCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionSupportCommand, ValidationResult>
{
    private readonly IValidator<DeleteQuestionSupportCommand> _validator;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public DeleteQuestionSupportCommandHandler(IValidator<DeleteQuestionSupportCommand> validator, IQuestionSupportRepository questionSupportRepository)
    {
        _validator = validator;
        _questionSupportRepository = questionSupportRepository;
    }

    public async Task<ValidationResult> HandleAsync(DeleteQuestionSupportCommand command, CancellationToken cancellationToken = default)
    {
        //To-Do: Deletar referencia em Question
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid) return ValidationResult;

        var questionSupport = await _questionSupportRepository.GetById(command.QuestionSupportId);
        if (questionSupport == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure(nameof(command.QuestionSupportId), "Question support not found."));
            return ValidationResult;
        }

        await _questionSupportRepository.Remove(questionSupport.Id);

        return ValidationResult;
    }
}
