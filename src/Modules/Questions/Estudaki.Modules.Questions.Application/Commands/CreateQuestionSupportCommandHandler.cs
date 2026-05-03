using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public class CreateQuestionSupportCommandHandler : CommandHandler, ICommandHandler<CreateQuestionSupportCommand, ValidationResult>
    {
        private readonly IValidator<CreateQuestionSupportCommand> _validator;
        private readonly IQuestionSupportRepository _questionSupportRepository;
        private readonly IPublicNoticeRepository _publicNoticeRepository;

        public CreateQuestionSupportCommandHandler(IValidator<CreateQuestionSupportCommand> validator,
            IQuestionSupportRepository questionSupportRepository,
            IPublicNoticeRepository publicNoticeRepository)
        {
            _validator = validator;
            _questionSupportRepository = questionSupportRepository;
            _publicNoticeRepository = publicNoticeRepository;
        }

        public async Task<ValidationResult> HandleAsync(CreateQuestionSupportCommand command, CancellationToken cancellationToken = default)
        {
            ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
            if(!ValidationResult.IsValid) {
                return ValidationResult;
            }

            var publicNotice = await _publicNoticeRepository.GetById(command.PublicNoticeId);
            if (publicNotice == null) 
            {
                ValidationResult.Errors.Add(new ValidationFailure("PublicNoticeId", "Public notice not found."));
                return ValidationResult;
            }

            var questionSupport = command.QuestionSupportDto.ToEntity();
            questionSupport.PublicNoticeId = command.PublicNoticeId;

            _questionSupportRepository.Add(questionSupport);

            return ValidationResult;
        }
    }
}
