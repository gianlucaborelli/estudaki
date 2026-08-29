using Estudaki.Modules.Ai.Application.DTOs;
using FluentValidation.Results;

namespace Estudaki.Modules.Ai.Application.Commands;

public class AIPromptCommandResult
{
    public ValidationResult ValidationResult { get; set; } = new();
    public AIPromptDto? Prompt { get; set; }

    public bool IsValid => ValidationResult.IsValid;
}
