using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList
{
    public class GetPublicNoticeListQueryHandler(
         IPublicNoticeRepository publicNoticeRepository,
         IStorageService storageService) : IQueryHandler<GetPublicNoticeListQuery, PagedResult<PublicNoticeDto>>
    {
        private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;
        private readonly IStorageService _storageService = storageService;

        public async Task<PagedResult<PublicNoticeDto>> HandleAsync(GetPublicNoticeListQuery query, CancellationToken cancellationToken = default)
        {
            var publicNotices = await _publicNoticeRepository.GetPublicNoticesByFilters(query.Page, query.PageSize, query.Search, query.Category, query.SortLabel, query.SortDirection);
            var pagedResult = new PagedResult<PublicNoticeDto>
            {
                Items = publicNotices.Item1.ToDtoList(_storageService),
                PageNumber = query.Page,
                PageSize = query.PageSize,
                TotalItems = publicNotices.Item2
            };
            return pagedResult;
        }
    }
}
