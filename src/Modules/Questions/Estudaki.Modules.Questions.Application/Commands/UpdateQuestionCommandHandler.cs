using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateQuestionCommandHandler : CommandHandler, ICommandHandler<UpdateQuestionCommand, ValidationResult>
{
    private readonly IValidator<UpdateQuestionCommand> _validator;
    private readonly IQuestionRepository _questionRepository;

    public UpdateQuestionCommandHandler(IValidator<UpdateQuestionCommand> validator, IQuestionRepository questionRepository)
    {
        _validator = validator;
        _questionRepository = questionRepository;
    }

    public async Task<ValidationResult> HandleAsync(UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid) return ValidationResult;

        var question = await _questionRepository.GetById(command.Question.QuestionId);
        if (question == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure("Question", "Question not found."));
            return ValidationResult;
        }

        question.Type = command.Question.QuestionType;
        question.MainArea = command.Question.MainArea;
        question.SubAreas = command.Question.SubAreas;
        question.QuestionSupports = command.Question.QuestionSupports.Select(s => s.Id).ToList();  
        question.QuestionContents = command.Question.QuestionContents;
        question.Choices = command.Question.Choices;

        await _questionRepository.Update(question);

        return ValidationResult;
    }
}
