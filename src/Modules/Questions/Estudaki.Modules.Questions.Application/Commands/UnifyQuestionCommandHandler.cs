using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UnifyQuestionCommandHandler : CommandHandler, ICommandHandler<UnifyQuestionCommand, ValidationResult>
{
    private readonly IValidator<UnifyQuestionCommand> _validator;
    private readonly IQuestionRepository _questionRepository;

    public UnifyQuestionCommandHandler(IValidator<UnifyQuestionCommand> validator, IQuestionRepository questionRepository)
    {
        _validator = validator;
        _questionRepository = questionRepository;
    }

    public async Task<ValidationResult> HandleAsync(UnifyQuestionCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid) return ValidationResult;

        var questions = await _questionRepository.GetManyById(command.QuestionsIds);

        if (questions == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure(nameof(command.QuestionsIds), "Some of the questions were not found."));
            return ValidationResult;
        }

        var questionToUnify = questions.FirstOrDefault(q => q.Id == command.QuestionsIds.First());        
        if (questionToUnify == null) {
            ValidationResult.Errors.Add(new ValidationFailure(nameof(command.QuestionsIds), "The question to unify was not found."));
            return ValidationResult;
        }
        var questionsToDelete = questions.Where(q => q.Id != questionToUnify.Id).ToList();
        var originalExam = questionToUnify.Exams
                                .Where(e => e.SourceExamId != null)
                                .FirstOrDefault();
        if (originalExam == null) 
        {
            ValidationResult.Errors.Add(new ValidationFailure(nameof(command.QuestionsIds), "The question to unify does not have an original exam."));
            return ValidationResult;
        }

        var exams = questions.SelectMany(q => q.Exams).ToList();

        foreach (var exam in exams) 
            if(exam.ExamId != originalExam.ExamId)
                exam.SourceExamId = string.Empty;
        
        questionToUnify.Exams = exams;

        await _questionRepository.Update(questionToUnify);

        foreach(var questionToDelete in questionsToDelete)
            await _questionRepository.Remove(questionToDelete.Id);

        return ValidationResult;
    }
}
