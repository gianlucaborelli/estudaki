using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class DeleteQuestionSupportCommandHandler : CommandHandler, ICommandHandler<DeleteQuestionSupportCommand, CommandResult>
{
    private readonly IValidator<DeleteQuestionSupportCommand> _validator;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public DeleteQuestionSupportCommandHandler(IValidator<DeleteQuestionSupportCommand> validator, IQuestionSupportRepository questionSupportRepository)
    {
        _validator = validator;
        _questionSupportRepository = questionSupportRepository;
    }

    public async Task<CommandResult> HandleAsync(DeleteQuestionSupportCommand command, CancellationToken cancellationToken = default)
    {
        //To-Do: Deletar referencia em Question
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var questionSupport = await _questionSupportRepository.GetById(command.QuestionSupportId);
        if (questionSupport == null)
        {
            AddError("Question support not found.");
            return Result();
        }

        await _questionSupportRepository.Remove(questionSupport.Id);

        return Result();
    }
}
