using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;

namespace Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;

public class GetFilterParametersQueryHandler : IQueryHandler<GetFilterParametersQuery, FilterParameters>
{
    private readonly IQuestionRepository _questionRepository;

    public GetFilterParametersQueryHandler(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<FilterParameters> HandleAsync(GetFilterParametersQuery query, CancellationToken cancellationToken = default)
    {
        return await _questionRepository.FindFilterParametersAsync(query.FilterParameters);
    }
}
