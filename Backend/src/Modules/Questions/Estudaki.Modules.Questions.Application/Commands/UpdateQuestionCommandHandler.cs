using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateQuestionCommandHandler : CommandHandler, ICommandHandler<UpdateQuestionCommand, CommandResult>
{
    private readonly IValidator<UpdateQuestionCommand> _validator;
    private readonly IQuestionRepository _questionRepository;

    public UpdateQuestionCommandHandler(IValidator<UpdateQuestionCommand> validator, IQuestionRepository questionRepository)
    {
        _validator = validator;
        _questionRepository = questionRepository;
    }

    public async Task<CommandResult> HandleAsync(UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var question = await _questionRepository.GetById(command.Question.QuestionId);
        if (question == null)
        {
            AddError("Question not found.");
            return Result();
        }

        var exam = question.Exams.FirstOrDefault(e => e.ExamId == command.Question.ExamId);
        if (exam == null) 
        {
            AddError("Exam not found for the question.");
            return Result();
        }

        question.Type = command.Question.QuestionType;
        question.MainArea = command.Question.MainArea;
        question.SubAreas = command.Question.SubAreas;
        question.QuestionSupports = command.Question.QuestionSupports.Select(s => s.Id).ToList();  
        question.QuestionContents = command.Question.QuestionContents;
        question.Choices = command.Question.Choices;        

        exam.QuestionNumber = command.Question.QuestionNumber;
        
        await _questionRepository.Update(question);

        return Result();
    }
}
