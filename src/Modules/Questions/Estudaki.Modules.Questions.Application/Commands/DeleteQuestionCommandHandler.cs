using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionCommand, ValidationResult>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IValidator<DeleteQuestionCommand> _validator;

    public DeleteQuestionCommandHandler(
        IQuestionRepository questionRepository, 
        IValidator<DeleteQuestionCommand> validator)
    {
        _questionRepository = questionRepository;
        _validator = validator;
    }

    public async Task<ValidationResult> HandleAsync(DeleteQuestionCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command);
        if(!ValidationResult.IsValid) return ValidationResult;

        var question = await _questionRepository.GetById(command.QuestionId);

        if (question == null) 
        { 
            ValidationResult.Errors.Add(new ValidationFailure("QuestionId", "Questão não encontrada."));
            return ValidationResult;
        }

        // Verificar se a questão está associada ao exame especificado
        var hasExam = question.Exams.Any(qe => qe.ExamId == command.ExamId);
        if (!hasExam)
        {
            ValidationResult.Errors.Add(new ValidationFailure("ExamId", "Questão não associada a esta prova."));
            return ValidationResult;
        }

        // Se a questão está associada apenas a este exame, remove a questão inteira
        if (question.Exams.Count == 1)
        {
            await _questionRepository.Remove(question.Id);
        }
        else
        {
            // Se está associada a múltiplos exames, remove apenas a associação com este exame
            question.Exams.RemoveAll(qe => qe.ExamId == command.ExamId);
            await _questionRepository.Update(question);
        }

        return ValidationResult;
    }
}
