using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.SearchQuestions;

public class SearchQuestionsPaginatedQueryHandler : IQueryHandler<SearchQuestionsPaginatedQuery, PageResult<QuestionDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IStorageService _storageService;
    private readonly StorageSettings _storageSettings;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IStorageService storageService,
        StorageSettings storageSettings)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _storageService = storageService;
        _storageSettings = storageSettings;
    }

    public async Task<PageResult<QuestionDto>> HandleAsync(SearchQuestionsPaginatedQuery query, CancellationToken cancellationToken = default)
    {
        var questionsPage = await _questionRepository.FindQuestionsPaginatedAsync(query.SearchParameters);

        var publicNoticeIds = questionsPage.Items
            .Where(q => !string.IsNullOrEmpty(q.PublicNoticeId))
            .Select(q => q.PublicNoticeId!)
            .Distinct()
            .ToList();

        var publicNotices = publicNoticeIds.Any()
            ? await _publicNoticeRepository.GetByIds(publicNoticeIds)
            : [];

        var publicNoticesDict = publicNotices.ToDictionary(pn => pn.Id!);

        var dtos = questionsPage.Items.Select(question =>
        {
            var publicNotice = !string.IsNullOrEmpty(question.PublicNoticeId) && publicNoticesDict.ContainsKey(question.PublicNoticeId)
                ? publicNoticesDict[question.PublicNoticeId]
                : null;

            return question.ToDto(publicNotice, _storageService, _storageSettings);
        }).ToList();

        return new PageResult<QuestionDto>
        {
            Items = dtos,
            PageNumber = questionsPage.PageNumber,
            PageSize = questionsPage.PageSize,
            TotalItems = questionsPage.TotalItems
        };
    }
}
