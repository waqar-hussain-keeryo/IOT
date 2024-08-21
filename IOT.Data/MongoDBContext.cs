using IOT.Entities.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace IOT.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
            _database = client.GetDatabase(configuration["DatabaseName"]);
        }

        public IMongoCollection<Users> Users => _database.GetCollection<Users>("Users");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Role");
    }
}
