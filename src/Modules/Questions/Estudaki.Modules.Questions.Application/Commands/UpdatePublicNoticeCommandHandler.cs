using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdatePublicNoticeCommandHandler : CommandHandler, ICommandHandler<UpdatePublicNoticeCommand, ValidationResult>
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

    public async Task<ValidationResult> HandleAsync(UpdatePublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid) {
            return ValidationResult;
        }

        var publicNotice = await _publicNoticeRepository.GetById(command.PublicNoticeDto.Id);

        if (publicNotice == null) 
        { 
            ValidationResult.Errors.Add(new ValidationFailure("PublicNotice", "Public notice not found."));
            return ValidationResult;
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

        return ValidationResult;
    }
}
