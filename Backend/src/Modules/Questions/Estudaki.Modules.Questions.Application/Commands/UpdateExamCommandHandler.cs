using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateExamCommandHandler : CommandHandler, ICommandHandler<UpdateExamCommand, CommandResult>
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

    public async Task<CommandResult> HandleAsync(UpdateExamCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success)
        {
            return Result();
        }

        var publicNotice = await _publicNoticeRepository.GetByExamId(command.Exam.Id);

        if (publicNotice == null)
        {
            AddError("Public notice not found for the given exam ID.");
            return Result();
        }

        var index = publicNotice.Exams
            .FindIndex(e => e.Id == command.Exam.Id);

        if (index == -1)
        {
            AddError("Exam not found in the public notice.");
            return Result();
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
        return Result();
    }
}
