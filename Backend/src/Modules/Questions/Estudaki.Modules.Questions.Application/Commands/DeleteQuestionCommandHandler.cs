using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionCommand, CommandResult>
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

    public async Task<CommandResult> HandleAsync(DeleteQuestionCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if(!CommandResult.Success) return Result();

        var question = await _questionRepository.GetById(command.QuestionId);

        if (question == null) 
        { 
            AddError("Questão não encontrada.");
            return Result();
        }

        // Verificar se a questão está associada ao exame especificado
        var hasExam = question.Exams.Any(qe => qe.ExamId == command.ExamId);
        if (!hasExam)
        {
            AddError("Questão não associada a esta prova.");
            return Result();
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

        return Result();
    }
}
