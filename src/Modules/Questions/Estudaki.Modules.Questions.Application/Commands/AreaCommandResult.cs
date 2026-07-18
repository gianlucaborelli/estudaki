using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class AreaCommandResult
{
    public ValidationResult ValidationResult { get; set; } = new();
    public AreaDto? Area { get; set; }

    public bool IsValid => ValidationResult.IsValid;
}
