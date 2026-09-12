using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateQuestionSupportCommandHandler : CommandHandler, ICommandHandler<UpdateQuestionSupportCommand, CommandResult>
{
    private readonly IValidator<UpdateQuestionSupportCommand> _validator;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public UpdateQuestionSupportCommandHandler(IValidator<UpdateQuestionSupportCommand> validator, IQuestionSupportRepository questionRepository)
    {
        _validator = validator;
        _questionSupportRepository = questionRepository;
    }

    public async Task<CommandResult> HandleAsync(UpdateQuestionSupportCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var questionSupport = await _questionSupportRepository.GetById(command.QuestionSupportDto.Id);

        if (questionSupport == null)
        {
            AddError("Question support not found.");
            return Result();
        }

        var updatedQuestionSupport = command.QuestionSupportDto.ToEntity();
        await _questionSupportRepository.Update(updatedQuestionSupport);

        return Result();
    }
}
