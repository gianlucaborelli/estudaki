using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly IDbContextFactory<QuestionsDbContext> _contextFactory;

    public AreaRepository(IDbContextFactory<QuestionsDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task AddAsync(Area area)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Areas.AddAsync(area);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Area area)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Areas.Update(area);
        await context.SaveChangesAsync();
    }

    public async Task<Area?> GetByIdAsync(string id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Areas.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<PagedResult<Area>> GetPaginatedAsync(AreaType type, string? name, int pageNumber, int pageSize)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var typeValue = type.ToString();

        var query = context.Areas
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
