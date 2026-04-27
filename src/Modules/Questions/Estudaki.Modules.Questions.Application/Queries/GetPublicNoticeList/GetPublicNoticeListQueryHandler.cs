using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeList
{
    public class GetPublicNoticeListQueryHandler(
         IPublicNoticeRepository publicNoticeRepository,
         IStorageService storageService) : IQueryHandler<GetPublicNoticeListQuery, List<PublicNoticeDto>>
    {
        private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;
        private readonly IStorageService _storageService = storageService;

        public async Task<List<PublicNoticeDto>> HandleAsync(GetPublicNoticeListQuery query, CancellationToken cancellationToken = default)
        {
            var publicNotices = await _publicNoticeRepository.GetPublicNoticesList();
            return publicNotices.ToDtoList(_storageService);
        }
    }
}
