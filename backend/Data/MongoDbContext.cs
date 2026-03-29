using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<TodoTask> Tasks { get; }
        IMongoCollection<User> Users { get; }
    }

    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDbConnection")
                ?? throw new InvalidOperationException("MongoDB connection string 'MongoDbConnection' not found in configuration.");
            
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("TodoDb");
        }

        public IMongoCollection<TodoTask> Tasks => _database.GetCollection<TodoTask>("tasks");
        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    }
}
