using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Models;
using MongoDB.Driver;

namespace Estudaki.Commons.Core.Data.Repository;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly IMongoContext Context;
    protected IMongoCollection<TEntity> DbSet;

    protected BaseRepository(IMongoContext context)
    {
        Context = context;
        DbSet = Context.GetCollection<TEntity>(typeof(TEntity).Name);
    }

    public virtual void Add(TEntity obj)
    {
        DbSet.InsertOne(obj);
    }

    public virtual async Task<TEntity> GetById(string id)
    {
        var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq(x => x.Id, id));
        return data.SingleOrDefault();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
        return all.ToList();
    }

    public virtual void Update(TEntity obj)
    {
        DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.Id), obj);
    }

    public virtual void Remove(Guid id)
    {
        DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id));
    }
}
