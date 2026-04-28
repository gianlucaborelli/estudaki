using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public class UploadPublicNoticeFilesCommandHandler : CommandHandler, ICommandHandler<UploadPublicNoticeFilesCommand, ValidationResult>
    {
        private readonly IValidator<UploadPublicNoticeFilesCommand> _validator;
        private readonly IPublicNoticeRepository _publicNoticeRepository;
        private readonly IStorageService _storageService;

        public UploadPublicNoticeFilesCommandHandler(IValidator<UploadPublicNoticeFilesCommand> validator, IPublicNoticeRepository publicNoticeRepository, IStorageService storageService) : base() 
        {
            _validator = validator;
            _publicNoticeRepository = publicNoticeRepository;
            _storageService = storageService;
        }

        public async Task<ValidationResult> HandleAsync(UploadPublicNoticeFilesCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            var publicNotice = await _publicNoticeRepository.GetById(command.publicNoticeId);

            if (publicNotice == null)
            {
                validationResult.Errors.Add(new ValidationFailure(nameof(command.publicNoticeId), "Public notice not found."));
                return validationResult;
            }            

            await _storageService.UploadFileAsync(
                command.examFile.OpenReadStream(), 
                publicNotice.GetExamFileName(), 
                command.examFile.ContentType);

            await _storageService.UploadFileAsync(
                command.answerKeyFile.OpenReadStream(),
                publicNotice.GetAnswerKeyFileName(),
                command.answerKeyFile.ContentType);

            publicNotice.HasAttachments = true;

            await _publicNoticeRepository.Update(publicNotice);

            return validationResult;
        }
    }
}
