using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.SearchQuestions;

public class SearchQuestionsPaginatedQueryHandler : IQueryHandler<SearchQuestionsPaginatedQuery, PageResult<QuestionWithNoticeDto>>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;

    public SearchQuestionsPaginatedQueryHandler(
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
    }

    public async Task<PageResult<QuestionWithNoticeDto>> HandleAsync(SearchQuestionsPaginatedQuery query, CancellationToken cancellationToken = default)
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

        var dtos = questionsPage.Items.Select(question => new QuestionWithNoticeDto
        {
            Question = question,
            PublicNotice = !string.IsNullOrEmpty(question.PublicNoticeId) && publicNoticesDict.ContainsKey(question.PublicNoticeId)
                ? publicNoticesDict[question.PublicNoticeId]
                : null
        }).ToList();

        return new PageResult<QuestionWithNoticeDto>
        {
            Items = dtos,
            PageNumber = questionsPage.PageNumber,
            PageSize = questionsPage.PageSize,
            TotalItems = questionsPage.TotalItems
        };
    }
}
