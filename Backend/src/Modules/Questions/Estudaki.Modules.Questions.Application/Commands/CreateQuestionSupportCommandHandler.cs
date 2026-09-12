using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public class CreateQuestionSupportCommandHandler : CommandHandler, ICommandHandler<CreateQuestionSupportCommand, CommandResult>
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

        public async Task<CommandResult> HandleAsync(CreateQuestionSupportCommand command, CancellationToken cancellationToken = default)
        {
            SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
            if(!CommandResult.Success) {
                return Result();
            }

            var publicNotice = await _publicNoticeRepository.GetById(command.PublicNoticeId);
            if (publicNotice == null) 
            {
                AddError("Public notice not found.");
                return Result();
            }

            var questionSupport = command.QuestionSupportDto.ToEntity();
            questionSupport.PublicNoticeId = command.PublicNoticeId;

            _questionSupportRepository.Add(questionSupport);

            return Result();
        }
    }
}
