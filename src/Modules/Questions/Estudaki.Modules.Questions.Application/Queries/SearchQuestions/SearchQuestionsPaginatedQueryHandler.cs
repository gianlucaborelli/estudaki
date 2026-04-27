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
    private readonly IQuestionSupportRepository _questionSupportRepository;
    private readonly IStorageService _storageService;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IQuestionSupportRepository questionSupportRepository,
        IStorageService storageService)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _questionSupportRepository = questionSupportRepository;
        _storageService = storageService;
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

        // Buscar todos os QuestionSupports necessários
        var allQuestionSupportIds = questionsPage.Items
            .Where(q => q.QuestionSupports != null && q.QuestionSupports.Any())
            .SelectMany(q => q.QuestionSupports)
            .Distinct()
            .ToList();

        var questionSupports = allQuestionSupportIds.Any()
            ? await _questionSupportRepository.GetByIds(allQuestionSupportIds)
            : [];

        var questionSupportsDict = questionSupports.ToDictionary(qs => qs.Id!);

        var dtos = questionsPage.Items.Select(question =>
        {
            var publicNotice = !string.IsNullOrEmpty(question.PublicNoticeId) && publicNoticesDict.ContainsKey(question.PublicNoticeId)
                ? publicNoticesDict[question.PublicNoticeId]
                : null;

            var supports = question.QuestionSupports != null && question.QuestionSupports.Any()
                ? question.QuestionSupports
                    .Where(id => questionSupportsDict.ContainsKey(id))
                    .Select(id => questionSupportsDict[id])
                    .ToList()
                : null;

            return question.ToDto(publicNotice, supports, _storageService);
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
