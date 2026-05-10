using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionCommand, ValidationResult>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IValidator<DeleteQuestionCommand> _validator;

    public DeleteQuestionCommandHandler(IQuestionRepository questionRepository, 
        IValidator<DeleteQuestionCommand> validator)
    {
        _questionRepository = questionRepository;
        _validator = validator;
    }
    public async Task<ValidationResult> HandleAsync(DeleteQuestionCommand command, CancellationToken cancellationToken = default)
    {
        //To-Do: Deletar ExamQuestion associada a questão
        ValidationResult = await _validator.ValidateAsync(command);
        if(!ValidationResult.IsValid) return ValidationResult;

        var question = await _questionRepository.GetById(command.QuestionId);

        if (question == null) 
        { 
            ValidationResult.Errors.Add(new ValidationFailure("QuestionId", "Questão não encontrada."));
            return ValidationResult;
        }

        await _questionRepository.Remove(question.Id);
        return ValidationResult;
    }
}
