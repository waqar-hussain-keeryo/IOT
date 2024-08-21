using IOT.Entities.Models;
using MongoDB.Driver;

namespace IOT.Data.Repositories
{
    public interface IGlobalAdminRepository
    {
        Task CreateUser(Users admin);
        Task<Users> GetUserByEmail(string email);
    }

    public class GlobalAdminRepository : IGlobalAdminRepository
    {
        private readonly MongoDBContext _context;

        public GlobalAdminRepository(MongoDBContext context)
        {
            _context = context;
        }

        public async Task CreateUser(Users admin)
        {
            await _context.Users.InsertOneAsync(admin);
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
