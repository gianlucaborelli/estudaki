using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public class UploadExamFilesCommandHandler : CommandHandler, ICommandHandler<UploadExamFilesCommand, ValidationResult>
    {
        private readonly IValidator<UploadExamFilesCommand> _validator;
        private readonly IPublicNoticeRepository _publicNoticeRepository;
        private readonly IStorageService _storageService;

        public UploadExamFilesCommandHandler(IValidator<UploadExamFilesCommand> validator, 
            IPublicNoticeRepository publicNoticeRepository, 
            IStorageService storageService) : base() 
        {
            _validator = validator;
            _publicNoticeRepository = publicNoticeRepository;
            _storageService = storageService;
        }

        public async Task<ValidationResult> HandleAsync(UploadExamFilesCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var publicNotice = await _publicNoticeRepository.GetById(command.examId);

            if (publicNotice == null)
            {
                validationResult.Errors.Add(new ValidationFailure(nameof(command.examId), "Public notice not found."));
                return validationResult;
            }            

            var examFile = await _storageService.UploadFileAsync(
                command.examFile.OpenReadStream(), 
                publicNotice.GetExamFileName(), 
                command.examFile.ContentType);

            var answerKeyFile = await _storageService.UploadFileAsync(
                command.answerKeyFile.OpenReadStream(),
                publicNotice.GetAnswerKeyFileName(),
                command.answerKeyFile.ContentType);

            var exam = publicNotice.Exams.FirstOrDefault(e => e.Id == command.examId);
            if (exam != null)
            {
                exam.ExamBookletUrl = examFile;
                exam.AnswerKeyUrl = answerKeyFile;
            }

            await _publicNoticeRepository.Update(publicNotice);

            return validationResult;
        }
    }
}
