using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateExamCommandHandler : CommandHandler, ICommandHandler<UpdateExamCommand, ValidationResult>
{
    private readonly IValidator<UpdateExamCommand> _validator;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IQuestionRepository _questionRepository;
    public UpdateExamCommandHandler(
        IValidator<UpdateExamCommand> validator, 
        IPublicNoticeRepository publicNoticeRepository, 
        IQuestionRepository questionRepository)
    {
        _validator = validator;
        _publicNoticeRepository = publicNoticeRepository;
        _questionRepository = questionRepository;
    }

    public async Task<ValidationResult> HandleAsync(UpdateExamCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(command, cancellationToken);
        if (!result.IsValid)
        {
            return result;
        }

        var publicNotice = await _publicNoticeRepository.GetByExamId(command.Exam.Id);

        if (publicNotice == null)
        {
            result.Errors.Add(new ValidationFailure(nameof(command.Exam.Id), "Public notice not found for the given exam ID."));
            return result;
        }

        var index = publicNotice.Exams
            .FindIndex(e => e.Id == command.Exam.Id);

        if (index == -1)
        {
            result.Errors.Add(
                new ValidationFailure(
                    nameof(command.Exam.Id),
                    "Exam not found in the public notice."));

            return result;
        }

        publicNotice.Exams[index] = command.Exam;

        await _publicNoticeRepository.Update(publicNotice);

        var examQuestion = await _questionRepository.GetByExamId(command.Exam.Id);

        foreach (var question in examQuestion)
        {
            var examQuestionToUpdate = question.Exams.FirstOrDefault(e => e.ExamId == command.Exam.Id);

            if (examQuestionToUpdate != null)
            {
                examQuestionToUpdate.Position = command.Exam.Position;
                examQuestionToUpdate.Phase = command.Exam.Phase;
                examQuestionToUpdate.Area = command.Exam.Area;
                examQuestionToUpdate.EducationLevel = command.Exam.EducationLevel;
                examQuestionToUpdate.ExamBookletUrl = command.Exam.ExamBookletUrl;
                examQuestionToUpdate.AnswerKeyUrl = command.Exam.AnswerKeyUrl;

                await _questionRepository.Update(question);
            }            
        }
        return ValidationResult;
    }
}
