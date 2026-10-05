using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using NanoidDotNet;

namespace Estudaki.Modules.Questions.Domain.Entities;

/// <summary>
/// Representa uma Área ou SubÁrea de conhecimento usada para classificar questões.
/// </summary>
public class Area : Entity
{
    private const int IdSize = 10;

    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static Area Create(string name, AreaType type)
    {
        return new Area
        {
            Id = Nanoid.Generate(size: IdSize),
            Name = name,
            Type = type.ToString(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
