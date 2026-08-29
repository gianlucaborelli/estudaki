using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.DTOs;

public class AreaDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AreaType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
