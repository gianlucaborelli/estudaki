using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.Interfaces;
using Estudaki.Modules.Ai.Domain.Entities;

namespace Estudaki.Modules.Ai.Application.Queries;

public class GetAllAIPromptsQueryHandler : IQueryHandler<GetAllAIPromptsQuery, List<AIPrompt>>
{
    private readonly IAiRepository _promptRepository;

    public GetAllAIPromptsQueryHandler(IAiRepository promptRepository)
    {
        _promptRepository = promptRepository;
    }

    public async Task<List<AIPrompt>> HandleAsync(GetAllAIPromptsQuery query, CancellationToken cancellationToken = default)
    {
        var prompts = await _promptRepository.GetAll();
        return prompts.ToList();
    }
}
