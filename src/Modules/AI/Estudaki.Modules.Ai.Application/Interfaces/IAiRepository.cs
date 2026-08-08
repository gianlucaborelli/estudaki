using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Ai.Domain.Entities;

namespace Estudaki.Modules.Ai.Application.Interfaces;

public interface IAiRepository : IRepository<AIPrompt>
{
    /// <summary>
    /// Busca um prompt ativo pelo seu nome único (ver <see cref="AIPromptNames"/>).
    /// </summary>
    Task<AIPrompt?> GetByNameAsync(string name);
}
