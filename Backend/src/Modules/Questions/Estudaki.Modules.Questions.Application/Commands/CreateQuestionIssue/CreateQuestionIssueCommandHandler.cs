using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands.CreateQuestionIssue;

public class CreateQuestionIssueCommandHandler : CommandHandler, ICommandHandler<CreateQuestionIssueCommand, CommandResult>
{
    private readonly IQuestionIssueRepository _questionIssueRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IValidator<CreateQuestionIssueCommand> _validator;

    public CreateQuestionIssueCommandHandler(
        IQuestionIssueRepository questionIssueRepository,
        IQuestionRepository questionRepository,
        IValidator<CreateQuestionIssueCommand> validator)
    {
        _questionIssueRepository = questionIssueRepository;
        _questionRepository = questionRepository;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(CreateQuestionIssueCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(_validator.Validate(command));
        if (!CommandResult.Success) return Result();

        var question = await _questionRepository.GetById(command.QuestionId);

        if(question == null)
        {
            AddError("Question not found.");
            return Result();
        }

        var questionIssue = QuestionIssue.Create(
            question.Id,
            command.UserName,
            command.UserEmail,
            command.CanBeReplied,
            command.Type,
            command.Description
        );

        _questionIssueRepository.Add(questionIssue);

        return Result();
    }
}
