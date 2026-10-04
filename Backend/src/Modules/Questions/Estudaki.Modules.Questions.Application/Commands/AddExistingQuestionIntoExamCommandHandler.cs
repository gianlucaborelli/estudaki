using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands;

public class AddExistingQuestionIntoExamCommandHandler : CommandHandler, ICommandHandler<AddExistingQuestionIntoExamCommand, CommandResult>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IValidator<AddExistingQuestionIntoExamCommand> _validator;

    public AddExistingQuestionIntoExamCommandHandler(
        IValidator<AddExistingQuestionIntoExamCommand> validator,
        IQuestionRepository questionRepository, 
        IPublicNoticeRepository publicNoticeRepository)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(AddExistingQuestionIntoExamCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult( _validator.Validate(command));
        if (!CommandResult.Success) return Result();

        var question = await _questionRepository.GetById(command.Question.QuestionId);
        if (question == null) 
        {
            AddError("Question not found.");
            return Result();
        }

        var publicNotice = await _publicNoticeRepository.GetByExamId(command.ExamId);
        var exam = publicNotice.Exams.FirstOrDefault(e => e.Id == command.ExamId);
        if (exam == null)
        {
            AddError("Exam not found.");
            return Result();
        }

        var questionExam = new QuestionExam
        {
            ExamId = command.ExamId,
            PublicNoticeId = publicNotice.Id,
            SourceExamId = "",
            QuestionNumber = 0,
            Year = publicNotice.Year,
            ExamCategory = publicNotice.ExamCategory,
            ExaminerOrganization = publicNotice.ExaminerOrganization,
            ContractingOrganization = publicNotice.ContractingOrganization,
            Position = exam.Position,
            Phase = exam.Phase,
            Area = exam.Area,
            EducationLevel = exam.EducationLevel,
            ExamBookletUrl = exam.ExamBookletUrl,
            AnswerKeyUrl = exam.AnswerKeyUrl
        };

        question.Exams.Add(questionExam);

        await _questionRepository.Update(question);

        return Result();
    }
}
