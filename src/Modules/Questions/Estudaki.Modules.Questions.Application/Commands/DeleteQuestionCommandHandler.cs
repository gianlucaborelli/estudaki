using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionCommand, ValidationResult>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IExamQuestionRepository _examQuestionRepository;
    private readonly IValidator<DeleteQuestionCommand> _validator;

    public DeleteQuestionCommandHandler(IQuestionRepository questionRepository, 
        IExamQuestionRepository examQuestionRepository,
        IValidator<DeleteQuestionCommand> validator)
    {
        _questionRepository = questionRepository;
        _examQuestionRepository = examQuestionRepository;
        _validator = validator;
    }
    public async Task<ValidationResult> HandleAsync(DeleteQuestionCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command);
        if(!ValidationResult.IsValid) return ValidationResult;

        var examQuestion = await _examQuestionRepository.GetByExamAndQuestion(command.ExamId, command.QuestionId);

        if (examQuestion == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure("ExamId", "Questão não associada a prova."));
            return ValidationResult;
        }

        var question = await _questionRepository.GetById(command.QuestionId);

        if (question == null) 
        { 
            ValidationResult.Errors.Add(new ValidationFailure("QuestionId", "Questão não encontrada."));
            return ValidationResult;
        }

        await _examQuestionRepository.Remove(examQuestion.Id);
        await _questionRepository.Remove(question.Id);
        return ValidationResult;
    }
}
