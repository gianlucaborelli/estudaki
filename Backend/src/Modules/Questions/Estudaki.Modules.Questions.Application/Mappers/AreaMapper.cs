using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class AreaMapper
{
    public static AreaDto ToDto(this Area area)
    {
        return new AreaDto
        {
            Id = area.Id,
            Name = area.Name,
            Type = Enum.Parse<AreaType>(area.Type),
            CreatedAt = area.CreatedAt
        };
    }

    public static List<AreaDto> ToDtoList(this IEnumerable<Area> areas)
    {
        return areas.Select(a => a.ToDto()).ToList();
    }
}
