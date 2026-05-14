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
        private readonly IQuestionRepository _questionRepository;
        private readonly IStorageService _storageService;

        public UploadExamFilesCommandHandler(IValidator<UploadExamFilesCommand> validator, 
            IPublicNoticeRepository publicNoticeRepository, 
            IQuestionRepository questionRepository,
            IStorageService storageService) : base() 
        {
            _validator = validator;
            _publicNoticeRepository = publicNoticeRepository;
            _questionRepository = questionRepository;
            _storageService = storageService;
        }

        public async Task<ValidationResult> HandleAsync(UploadExamFilesCommand command, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid) return validationResult;

            var publicNotice = await _publicNoticeRepository.GetById(command.publicNoticeId);
            var exam = publicNotice?.Exams.FirstOrDefault(e => e.Id == command.examId);

            if (publicNotice == null)
            {
                validationResult.Errors
                    .Add(new ValidationFailure(nameof(command.publicNoticeId), "Public notice not found."));
                return validationResult;
            }            

            var examFile = await _storageService.UploadFileAsync(
                command.examFile.OpenReadStream(), 
                publicNotice.BuildExamFilePath(command.examId), 
                command.examFile.ContentType);

            var answerKeyFile = await _storageService.UploadFileAsync(
                command.answerKeyFile.OpenReadStream(),
                publicNotice.BuildAnswerKeyPath(command.examId),
                command.answerKeyFile.ContentType);
            
            if (exam != null)
            {
                exam.ExamBookletUrl = examFile;
                exam.AnswerKeyUrl = answerKeyFile;
            }

            await _publicNoticeRepository.Update(publicNotice);

            var questions = await _questionRepository.GetByExamId(command.examId);

            foreach (var question in questions)
            {
                var questionExam = question.Exams.FirstOrDefault(e => e.ExamId == command.examId);
                
                questionExam!.ExamBookletUrl = examFile;
                questionExam!.AnswerKeyUrl = answerKeyFile;

                await _questionRepository.Update(question);
            }

            return validationResult;
        }
    }
}
