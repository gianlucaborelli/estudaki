using MongoDB.Driver;

namespace ProvaOnline.Data.Context
{
    public interface IMongoContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
