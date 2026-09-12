using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class AddNewQuestionIntoExamCommandHandler : CommandHandler, ICommandHandler<AddNewQuestionIntoExamCommand, CommandResult>
{
    private readonly IValidator<AddNewQuestionIntoExamCommand> _validator;
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public AddNewQuestionIntoExamCommandHandler(
        IValidator<AddNewQuestionIntoExamCommand> validator, 
        IQuestionRepository questionRepository, 
        IPublicNoticeRepository publicNoticeRepository)
    {
        _validator = validator;
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
    }
    public async Task<CommandResult> HandleAsync(AddNewQuestionIntoExamCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var publicNotice = await _publicNoticeRepository.GetById(command.Question.PublicNoticeId);
        if (publicNotice == null)
        {
            AddError("Public notice not found.");
            return Result();
        }

        var exam = publicNotice.Exams.FirstOrDefault(e => e.Id == command.Question.ExamId);
        if (exam == null)
        {
            AddError("Exam not found for the question.");
            return Result();
        }

        var question = new Question();                

        question.Type = command.Question.QuestionType;
        question.MainArea = command.Question.MainArea;
        question.SubAreas = command.Question.SubAreas;
        question.QuestionSupports = command.Question.QuestionSupports.Select(s => s.Id).ToList();
        question.QuestionContents = command.Question.QuestionContents;
        question.Choices = command.Question.Choices;

        var examQuestion = QuestionExam.Create(exam, publicNotice);
        examQuestion.QuestionNumber = command.Question.QuestionNumber;
        question.Exams.Add(examQuestion);

        _questionRepository.Add(question);

        return Result();
    }
}
