using FluentValidation.Results;

namespace Estudaki.Commons.Core.CQRS;

public abstract class CommandHandler
{
    protected CommandResult CommandResult { get; set; }

    protected CommandHandler()
    {
        CommandResult = new CommandResult();
    }

    protected void AddError(string mensagem)
    {
        CommandResult.ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
    }

    protected CommandResult Result()
    {
        return CommandResult;
    }

    protected CommandResult Result(object data)
    {
        CommandResult.Data = data;

        return CommandResult;
    }

    protected void SetValidationResult(ValidationResult validationResult)
    {
        CommandResult.ValidationResult = validationResult;
    }
}

public class CommandResult
{
    public bool Success => !ValidationResult.Errors.Any();

    public object? Data { get; internal set; }

    public ValidationResult ValidationResult { get; internal set; } = new();
}
