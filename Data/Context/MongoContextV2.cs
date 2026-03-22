using MongoDB.Driver;

namespace ProvaOnline.Data.Context
{
    public class MongoContextV2 : IMongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContextV2(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("ProvaOnlineV2");
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            try
            {
                return _database.GetCollection<T>(name);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                throw;

            }
        }
    }
}
