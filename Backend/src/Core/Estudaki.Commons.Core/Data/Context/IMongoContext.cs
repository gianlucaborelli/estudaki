using MongoDB.Driver;

namespace Estudaki.Commons.Core.Data.Context;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>();
}
