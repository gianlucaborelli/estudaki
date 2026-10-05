using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Commons.Core.Models;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class AreaRepository :EfRepositoryBase<Area>, IAreaRepository
{
    public AreaRepository(QuestionsDbContext context) : base(context)
    {       
    }

    public async Task AddAsync(Area area)
    {
        await _dbSet.AddAsync(area);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Area area)
    {
        _dbSet.Update(area);
        await _context.SaveChangesAsync();
    }

    public async Task<Area?> GetByIdAsync(string id)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<PagedResult<Area>> GetPaginatedAsync(AreaType type, string? name, int pageNumber, int pageSize)
    {
        var typeValue = type.ToString();

        var query = _dbSet
            .Where(a => a.Type == typeValue);

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{name}%"));
        }

        var totalItems = await query.LongCountAsync();

        var items = await query
            .OrderBy(a => a.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Area>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}
