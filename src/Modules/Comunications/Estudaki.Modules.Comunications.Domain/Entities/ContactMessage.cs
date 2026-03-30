using Estudaki.Commons.Core.Models;

namespace Estudaki.Modules.Comunications.Domain.Entities;

public sealed class ContactMessage: Entity
{
    public string? UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool CanBeReplied { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    private ContactMessage() { }

    public ContactMessage( string name, string email, string message, bool canBeReplied = false, string? userId = null)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Message = message;
        CanBeReplied = canBeReplied;
        CreatedAt = DateTime.UtcNow;
    }
}
