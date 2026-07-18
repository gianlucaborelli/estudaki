using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetAreasPaginated;

public class GetAreasPaginatedQueryHandler : IQueryHandler<GetAreasPaginatedQuery, PagedResult<AreaDto>>
{
    private readonly IAreaRepository _areaRepository;

    public GetAreasPaginatedQueryHandler(IAreaRepository areaRepository)
    {
        _areaRepository = areaRepository;
    }

    public async Task<PagedResult<AreaDto>> HandleAsync(GetAreasPaginatedQuery query, CancellationToken cancellationToken = default)
    {
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var result = await _areaRepository.GetPaginatedAsync(query.Type, query.Name, pageNumber, pageSize);

        return new PagedResult<AreaDto>
        {
            Items = result.Items.ToDtoList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
