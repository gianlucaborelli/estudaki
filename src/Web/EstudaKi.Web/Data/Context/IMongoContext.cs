using MongoDB.Driver;

namespace EstudaKi.Web.Data.Context
{
    public interface IMongoContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
