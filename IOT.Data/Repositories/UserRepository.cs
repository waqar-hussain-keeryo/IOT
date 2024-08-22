using IOT.Entities.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using IOT.Business.Services;

namespace IOT.Data.Repositories
{
    public interface IUserRepository
    {
        Task<Users> CreateUser(Users user);
        Task<IEnumerable<Users>> GetAllUsers();
        Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByCustomerId(Guid customerId, int pageNumber, int pageSize);
        Task<Users> GetUserByEmail(string email);
        Task<Users> GetUserById(Guid userId);
        Task<Users> GetUserByRoleId(Guid roleId);
        Task<Users> UpdateUser(Guid userId, Users updatedUser);
        Task DeleteUser(Guid userId);
        Task<Role> GetRoleByName(string roleName);
        Task<Role> GetRoleById(Guid roleId);
        Task<Guid> CreateRole(Role role);
    }

    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;

        public UserRepository(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<Users> CreateUser(Users user)
        {
            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            return await _context.Users.Find(_ => true).ToListAsync();
        }

        public async Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByCustomerId(Guid customerId, int pageNumber, int pageSize)
        {
            // Validate parameters
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10; // Default page size if less than 1

            // Get the collection from the database
            var collection = _context.GetCollection<Users>("Users");

            // Query to get the total number of records
            var totalRecords = await collection.CountDocumentsAsync(u => u.CustomerID == customerId);

            // Query to get the paginated users
            var users = await collection.Find(u => u.CustomerID == customerId)
                                        .Skip((pageNumber - 1) * pageSize)  // Skip records for previous pages
                                        .Limit(pageSize)                    // Limit records for the current page
                                        .ToListAsync();

            return (users, (int)totalRecords);
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserById(Guid userId)
        {
            return await _context.Users.Find(u => u.UserID == userId).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByRoleId(Guid roleId)
        {
            return await _context.Users.Find(u => u.RoleID == roleId).FirstOrDefaultAsync();
        }

        public async Task<Users> UpdateUser(Guid userId, Users updatedUser)
        {
            await _context.Users.ReplaceOneAsync(u => u.UserID == userId, updatedUser);
            return updatedUser;
        }

        public async Task DeleteUser(Guid userId)
        {
            await _context.Users.DeleteOneAsync(u => u.UserID == userId);
        }

        public async Task<Role> GetRoleById(Guid roleId)
        {
            return await _context.Roles.Find(r => r.RoleID == roleId).FirstOrDefaultAsync();
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return await _context.Roles.Find(r => r.RoleName == roleName).FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateRole(Role role)
        {
            if (role.RoleID == Guid.Empty)
            {
                role.RoleID = Guid.NewGuid();
            }

            await _context.Roles.InsertOneAsync(role);

            return role.RoleID;
        }
    }
}
