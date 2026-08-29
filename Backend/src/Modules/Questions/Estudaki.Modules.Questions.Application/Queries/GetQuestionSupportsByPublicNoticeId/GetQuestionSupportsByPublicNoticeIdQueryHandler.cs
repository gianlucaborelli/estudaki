using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetQuestionSupportsByPublicNoticeId;

public class GetQuestionSupportsByPublicNoticeIdQueryHandler(
    IQuestionSupportRepository questionSupportRepository) 
    : IQueryHandler<GetQuestionSupportsByPublicNoticeIdQuery, List<QuestionSupportDto>>
{
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;


    public async Task<List<QuestionSupportDto>> HandleAsync(GetQuestionSupportsByPublicNoticeIdQuery query, CancellationToken cancellationToken = default)
    {
        var result = await _questionSupportRepository.GetByPublicNoticeId(query.PublicNoticeId);

        if (result == null)
            return [];

        var questionSupports = result.ToDtoList();
        return questionSupports;
    }
}
