using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class PublicNoticeRepository : BaseRepository<PublicNotice>, IPublicNoticeRepository
{
    public PublicNoticeRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<List<PublicNotice>> GetPublicNoticesList()
    {
        return await DbSet.Find(_ => true).ToListAsync();
    }

    public async Task<(List<PublicNotice>, long)> GetPublicNoticesByFilters(
        int page, 
        int pageSize, 
        string? search, 
        string? category, 
        string? sortLabel, 
        string? sortDirection)
    {
        var builder = Builders<PublicNotice>.Filter;

        var filters = new List<FilterDefinition<PublicNotice>>();

        // Pesquisa
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            filters.Add(
                builder.Or(
                    builder.Regex(
                        x => x.ExaminerOrganization,
                        new BsonRegularExpression(search, "i")),

                    builder.Regex(
                        x => x.ContractingOrganization,
                        new BsonRegularExpression(search, "i"))
                )
            );
        }

        // Categoria
        if (!string.IsNullOrWhiteSpace(category))
        {
            filters.Add(
                builder.Eq(x => x.ExamCategory, category)
            );
        }

        var mongoFilter = filters.Count > 0
            ? builder.And(filters)
            : builder.Empty;

        var totalItems = await DbSet.CountDocumentsAsync(mongoFilter);

        var sort = GetSortDefinition(sortLabel, sortDirection);

        page = Math.Max(page, 0);
        pageSize = Math.Max(pageSize, 1);

        var items = await DbSet
            .Find(mongoFilter)
            .Sort(sort)
            .Skip(page * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    private static SortDefinition<PublicNotice> GetSortDefinition(
    string sortLabel, string sortDirection)
    {
        var builder = Builders<PublicNotice>.Sort;

        var descending = string.Equals(
            sortDirection,
            "Descending",
            StringComparison.OrdinalIgnoreCase);

        return sortLabel switch
        {
            "ExaminerOrganization" => descending
                ? builder.Descending(x => x.ExaminerOrganization)
                : builder.Ascending(x => x.ExaminerOrganization),

            "ContractingOrganization" => descending
                ? builder.Descending(x => x.ContractingOrganization)
                : builder.Ascending(x => x.ContractingOrganization),

            "Year" => descending
                ? builder.Descending(x => x.Year)
                : builder.Ascending(x => x.Year),

            "Number" => descending
                ? builder.Descending(x => x.Number)
                : builder.Ascending(x => x.Number),

            "ExamCategory" => descending
                ? builder.Descending(x => x.ExamCategory)
                : builder.Ascending(x => x.ExamCategory),                

            "CreatedAt" => descending
                ? builder.Descending(x => x.CreatedAt)
                : builder.Ascending(x => x.CreatedAt),

            _ => builder.Descending(x => x.CreatedAt)
        };
    }
    

    public async Task<List<PublicNotice>> GetByIds(List<string> ids)
    {
        if (ids == null || ids.Count == 0)
            return [];

        var filter = Builders<PublicNotice>.Filter.In(p => p.Id, ids);
        return await DbSet.Find(filter).ToListAsync();
    }

    public async Task<PublicNotice> GetPublicNoticeByExamId(string examId)
    {
        var filter = Builders<PublicNotice>.Filter.ElemMatch(p => p.Exams, e => e.Id == examId);
        return await DbSet.Find(filter).FirstOrDefaultAsync();
    }

    public Task<PublicNotice> GetByExamId(string examId)
    {
        var filter = Builders<PublicNotice>.Filter.ElemMatch(p => p.Exams, e => e.Id == examId);
        return DbSet.Find(filter).FirstOrDefaultAsync();
    }
}
