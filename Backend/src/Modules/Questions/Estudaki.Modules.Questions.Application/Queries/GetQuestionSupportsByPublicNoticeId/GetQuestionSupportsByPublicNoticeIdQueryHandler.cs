using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;

public class GetQuestionSupportsByPublicNoticeIdQueryHandler(
    IQuestionSupportRepository questionSupportRepository) 
    : IQueryHandler<GetQuestionSupportsByPublicNoticeIdQuery, PagedResult<QuestionSupportDto>>
{
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;

    public async Task<PagedResult<QuestionSupportDto>> HandleAsync(GetQuestionSupportsByPublicNoticeIdQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _questionSupportRepository.GetByPublicNoticeIdPagedAsync(query.PublicNoticeId, query.Page, query.PageSize, query.SortLabel, query.SortDirection);
        var pagedResult = new PagedResult<QuestionSupportDto>
        {
            Items = result.items.ToDtoList(),
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalItems = result.totalItems
        };
        return pagedResult;
    }
}
