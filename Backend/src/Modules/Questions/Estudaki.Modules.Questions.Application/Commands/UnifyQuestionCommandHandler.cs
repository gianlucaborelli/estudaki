using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UnifyQuestionCommandHandler : CommandHandler, ICommandHandler<UnifyQuestionCommand, CommandResult>
{
    private readonly IValidator<UnifyQuestionCommand> _validator;
    private readonly IQuestionRepository _questionRepository;

    public UnifyQuestionCommandHandler(IValidator<UnifyQuestionCommand> validator, IQuestionRepository questionRepository)
    {
        _validator = validator;
        _questionRepository = questionRepository;
    }

    public async Task<CommandResult> HandleAsync(UnifyQuestionCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var questions = await _questionRepository.GetManyById(command.QuestionsIds);

        if (questions == null)
        {
            AddError("Some of the questions were not found.");
            return Result();
        }

        var questionToUnify = questions.FirstOrDefault(q => q.Id == command.QuestionsIds.First());        
        if (questionToUnify == null) {
            AddError("The question to unify was not found.");
            return Result();
        }
        var questionsToDelete = questions.Where(q => q.Id != questionToUnify.Id).ToList();
        var originalExam = questionToUnify.Exams
                                .Where(e => e.SourceExamId != null)
                                .FirstOrDefault();
        if (originalExam == null) 
        {
            AddError("The question to unify does not have an original exam.");
            return Result();
        }

        var exams = questions.SelectMany(q => q.Exams).ToList();

        foreach (var exam in exams) 
            if(exam.ExamId != originalExam.ExamId)
                exam.SourceExamId = string.Empty;
        
        questionToUnify.Exams = exams;

        await _questionRepository.Update(questionToUnify);

        foreach(var questionToDelete in questionsToDelete)
            await _questionRepository.Remove(questionToDelete.Id);

        return Result();
    }
}
