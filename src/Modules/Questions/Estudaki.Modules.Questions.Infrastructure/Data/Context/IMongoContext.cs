using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Context;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}
