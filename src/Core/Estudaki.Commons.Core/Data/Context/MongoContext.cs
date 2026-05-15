using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Estudaki.Commons.Core.Data.Context;

public class MongoContext : IMongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IMongoClient client)
    {
        var databaseName = "ProvaOnlineV2";
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>()
    {
        var collectionName = typeof(T)
             .GetCustomAttributes(typeof(CollectionNameAttribute), false)
             .Cast<CollectionNameAttribute>()
             .FirstOrDefault()?.Name
             ?? typeof(T).Name;

        return _database.GetCollection<T>(collectionName);
    }
}
