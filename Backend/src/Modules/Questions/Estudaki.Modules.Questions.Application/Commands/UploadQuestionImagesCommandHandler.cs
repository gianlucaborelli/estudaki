using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Extensions;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public class UploadQuestionImagesCommandHandler : CommandHandler, ICommandHandler<UploadQuestionImagesCommand, CommandResult>
    {
        private readonly IValidator<UploadQuestionImagesCommand> _validator;
        private readonly IPublicNoticeRepository _publicNoticeRepository;
        private readonly IStorageService _storageService;

        public UploadQuestionImagesCommandHandler(IValidator<UploadQuestionImagesCommand> validator, IPublicNoticeRepository publicNoticeRepository, IStorageService storageService) : base()
        {
            _validator = validator;
            _publicNoticeRepository = publicNoticeRepository;
            _storageService = storageService;
        }

        public async Task<CommandResult> HandleAsync(UploadQuestionImagesCommand command, CancellationToken cancellationToken = default)
        {
            SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
            if (!CommandResult.Success) return Result();


            var publicNotice = await _publicNoticeRepository.GetById(command.PublicNoticeId);

            if (publicNotice == null)
            {
                AddError("Public notice not found.");
                return Result();
            }

            var file = command.File;
            var extension = file.ContentType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/svg+xml" => ".svg",
                _ => throw new ValidationException($"Unsupported file type: {file.ContentType}")
            };

            // Gerar GUID para o arquivo
            var guid = Guid.NewGuid().ToString();
            var newFileName = $"{publicNotice.GetImagesFolder()}/{guid}{extension}";

            await _storageService.UploadFileAsync(
                file.OpenReadStream(),
                newFileName,
                file.ContentType);

            return Result();
        }
    }
}
