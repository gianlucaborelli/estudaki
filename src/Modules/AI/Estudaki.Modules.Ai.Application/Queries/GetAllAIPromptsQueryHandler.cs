using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Application.DTOs;
using Estudaki.Modules.Ai.Application.Interfaces;

namespace Estudaki.Modules.Ai.Application.Queries;

public class GetAllAIPromptsQueryHandler : IQueryHandler<GetAllAIPromptsQuery, List<AIPromptDto>>
{
    private readonly IAiRepository _promptRepository;

    public GetAllAIPromptsQueryHandler(IAiRepository promptRepository)
    {
        _promptRepository = promptRepository;
    }

    public async Task<List<AIPromptDto>> HandleAsync(GetAllAIPromptsQuery query, CancellationToken cancellationToken = default)
    {
        var prompts = await _promptRepository.GetAll();
        return prompts.Select(AIPromptDto.FromEntity).ToList();
    }
}
