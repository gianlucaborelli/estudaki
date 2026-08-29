using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class AddNewQuestionIntoExamCommandHandler : CommandHandler, ICommandHandler<AddNewQuestionIntoExamCommand, ValidationResult>
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
    public async Task<ValidationResult> HandleAsync(AddNewQuestionIntoExamCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid) return ValidationResult;

        var publicNotice = await _publicNoticeRepository.GetById(command.Question.PublicNoticeId);
        if (publicNotice == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure("PublicNoticeId", "Public notice not found."));
            return ValidationResult;
        }

        var exam = publicNotice.Exams.FirstOrDefault(e => e.Id == command.Question.ExamId);
        if (exam == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure("ExamId", "Exam not found for the question."));
            return ValidationResult;
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

        return ValidationResult;
    }
}
