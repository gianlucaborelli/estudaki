using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Ai.Domain.Entities;

namespace Estudaki.Modules.Ai.Application.Queries;

/// <summary>
/// Busca todos os prompts de IA cadastrados.
/// </summary>
public record GetAllAIPromptsQuery : IQuery<List<AIPrompt>>;
