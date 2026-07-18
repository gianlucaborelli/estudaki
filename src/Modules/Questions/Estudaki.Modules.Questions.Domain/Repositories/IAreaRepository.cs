using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Domain.Repositories;

public interface IAreaRepository
{
    Task AddAsync(Area area);
    Task UpdateAsync(Area area);
    Task<Area?> GetByIdAsync(string id);
    Task<PagedResult<Area>> GetPaginatedAsync(AreaType type, string? name, int pageNumber, int pageSize);
}
