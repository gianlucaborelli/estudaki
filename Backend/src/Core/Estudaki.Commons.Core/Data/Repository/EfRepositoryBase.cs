using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Estudaki.Commons.Core.Data.Repository;

/// <summary>
/// Repositório genérico base para Entity Framework.
/// Fornece operações CRUD padrão para entidades que herdam de <see cref="Entity"/>.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade que deve herdar de Entity</typeparam>
public class EfRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly EfContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public EfRepositoryBase(EfContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Adiciona uma nova entidade ao contexto.
    /// </summary>
    /// <param name="obj">Entidade a ser adicionada</param>
    public void Add(TEntity obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        _dbSet.Add(obj);
    }

    /// <summary>
    /// Obtém todas as entidades do banco de dados.
    /// </summary>
    /// <returns>Coleção de todas as entidades</returns>
    public async Task<IEnumerable<TEntity>> GetAll()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Obtém uma entidade pelo ID.
    /// </summary>
    /// <param name="id">ID da entidade</param>
    /// <returns>Entidade encontrada ou null se não existir</returns>
    public async Task<TEntity> GetById(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    /// <param name="obj">Entidade a ser atualizada</param>
    public async Task Update(TEntity obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        _dbSet.Update(obj);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Remove uma entidade pelo ID.
    /// </summary>
    /// <param name="id">ID da entidade a ser removida</param>
    public async Task Remove(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var entity = await GetById(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }
}
