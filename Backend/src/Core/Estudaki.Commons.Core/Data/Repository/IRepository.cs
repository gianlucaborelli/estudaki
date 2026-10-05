using Estudaki.Commons.Core.Models;
namespace Estudaki.Commons.Core.Data.Repository;

public interface IRepository<TEntity> where TEntity : Entity
{
    void Add(TEntity obj);
    Task<TEntity> GetById(string id);
    Task<IEnumerable<TEntity>> GetAll();
    Task Update(TEntity obj);
    Task Remove(string id);
}
