using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Extensions;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetImageListByPublicNoticeId;

public class GetImageListByPublicNoticeIdQueryHandler : IQueryHandler<GetImageListByPublicNoticeIdQuery, List<string>>
{
    private readonly IStorageService _storageService;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public GetImageListByPublicNoticeIdQueryHandler(IStorageService storageService, IPublicNoticeRepository publicNoticeRepository)
    {
        _storageService = storageService;
        _publicNoticeRepository = publicNoticeRepository;
    }

    public async Task<List<string>> HandleAsync(GetImageListByPublicNoticeIdQuery query, CancellationToken cancellationToken = default)
    {
        var publicNotice = await _publicNoticeRepository.GetById(query.PublicNoticeId);
        if (publicNotice == null) return [];

        var path = publicNotice.GetImagesFolder();

        if (path == null) return [];

        var imageList = await _storageService.ListFilesAsync(path);

        var baseUrl = _storageService.GetFileUrl();

        imageList = imageList
            .Select(image => $"{baseUrl}/{image}")
            .ToList();

        return imageList;
    }
}
