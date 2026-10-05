using Estudaki.Commons.Core.Data;
using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Ai.Domain.Entities;

[CollectionName("ai_prompts")]
public sealed class AIPrompt : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private AIPrompt() { }

    public AIPrompt(string name, string content, string? description = null)
    {
        Name = name;
        Content = content;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content, string? description = null)
    {
        Content = content;
        Description = description ?? Description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
