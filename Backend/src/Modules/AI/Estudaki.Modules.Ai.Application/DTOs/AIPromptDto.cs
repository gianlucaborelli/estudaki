using Estudaki.Modules.Ai.Domain.Entities;

namespace Estudaki.Modules.Ai.Application.DTOs;

/// <summary>
/// DTO utilizado para comunicação entre o front-end (Web) e os commands/queries do módulo de IA.
/// </summary>
public class AIPromptDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static AIPromptDto FromEntity(AIPrompt prompt)
    {
        return new AIPromptDto
        {
            Id = prompt.Id,
            Name = prompt.Name,
            Content = prompt.Content,
            Description = prompt.Description,
            IsActive = prompt.IsActive,
            CreatedAt = prompt.CreatedAt,
            UpdatedAt = prompt.UpdatedAt,
        };
    }
}
