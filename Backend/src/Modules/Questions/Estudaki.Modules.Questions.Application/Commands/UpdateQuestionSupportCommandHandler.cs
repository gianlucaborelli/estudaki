using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UpdateQuestionSupportCommandHandler : CommandHandler, ICommandHandler<UpdateQuestionSupportCommand, ValidationResult>
{
    private readonly IValidator<UpdateQuestionSupportCommand> _validator;
    private readonly IQuestionSupportRepository _questionSupportRepository;

    public UpdateQuestionSupportCommandHandler(IValidator<UpdateQuestionSupportCommand> validator, IQuestionSupportRepository questionRepository)
    {
        _validator = validator;
        _questionSupportRepository = questionRepository;
    }

    public async Task<ValidationResult> HandleAsync(UpdateQuestionSupportCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if(!ValidationResult.IsValid)
        {
            return ValidationResult;
        }

        var questionSupport = await _questionSupportRepository.GetById(command.QuestionSupportDto.Id);

        if (questionSupport == null)
        {
            ValidationResult.Errors.Add(new ValidationFailure("QuestionSupport", "Question support not found."));
            return ValidationResult;
        }

        var updatedQuestionSupport = command.QuestionSupportDto.ToEntity();
        await _questionSupportRepository.Update(updatedQuestionSupport);

        return ValidationResult;
    }
}
