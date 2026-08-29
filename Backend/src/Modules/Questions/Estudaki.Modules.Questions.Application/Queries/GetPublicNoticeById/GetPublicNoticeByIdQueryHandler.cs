using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetPublicNoticeById;
public class GetPublicNoticeByIdQueryHandler(
    IPublicNoticeRepository publicNoticeRepository,
     IStorageService storageService) : IQueryHandler<GetPublicNoticeByIdQuery, PublicNoticeDto?>
{
    private readonly IPublicNoticeRepository _publicNoticeRepository = publicNoticeRepository;
    private readonly IStorageService _storageService = storageService;
    public async Task<PublicNoticeDto?> HandleAsync(GetPublicNoticeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var publicNotice = await _publicNoticeRepository.GetById(query.Id);
        if (publicNotice == null)
        {
            return null;
        }
        return publicNotice.ToDto(_storageService);
    }
}
