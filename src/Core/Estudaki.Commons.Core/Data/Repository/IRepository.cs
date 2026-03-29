using Estudaki.Commons.Core.Models;
namespace Estudaki.Commons.Core.Data.Repository;

public interface IRepository<TEntity> where TEntity : Entity
{
    void Add(TEntity obj);
    Task<TEntity> GetById(string id);
    Task<IEnumerable<TEntity>> GetAll();
    void Update(TEntity obj);
    void Remove(Guid id);
}
