using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdatePublicNoticeCommandHandler : CommandHandler, ICommandHandler<UpdatePublicNoticeCommand, CommandResult>
{
    private readonly IValidator<UpdatePublicNoticeCommand> _validator;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IQuestionRepository _questionRepository;

    public UpdatePublicNoticeCommandHandler(IValidator<UpdatePublicNoticeCommand> validator,
        IPublicNoticeRepository publicNoticeRepository,
        IQuestionRepository questionRepository)
    {
        _validator = validator;
        _publicNoticeRepository = publicNoticeRepository;
        _questionRepository = questionRepository;
    }

    public async Task<CommandResult> HandleAsync(UpdatePublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) {
            return Result();
        }

        var publicNotice = await _publicNoticeRepository.GetById(command.PublicNoticeDto.Id);

        if (publicNotice == null) 
        { 
            AddError("Public notice not found.");
            return Result();
        }

        var updatedPublicNotice = command.PublicNoticeDto.ToEntity();
        await _publicNoticeRepository.Update(updatedPublicNotice);

        var questions = await _questionRepository.GetByPublicNoticeId(command.PublicNoticeDto.Id);

        foreach (var question in questions)
        {
            var questionExam = question.Exams;

            foreach (var exam in questionExam!)
            {
                exam.Year = updatedPublicNotice.Year;
                exam.ExamCategory = updatedPublicNotice.ExamCategory;
                exam.ExaminerOrganization = updatedPublicNotice.ExaminerOrganization;
                exam.ContractingOrganization = updatedPublicNotice.ContractingOrganization;                
            }

            await _questionRepository.Update(question);
        }

        return Result();
    }
}
