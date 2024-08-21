using IOT.Entities.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using IOT.Business.Services;

namespace IOT.Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(Users user);
        Task<Users> GetUserByEmail(string email);
        Task<Users> GetUserById(Guid userId);
        Task UpdateUser(Guid userId, Users updatedUser);
        Task DeleteUser(Guid userId);
        Task<Role> GetRoleByName(string roleName);
        Task<Role> GetRoleById(Guid roleId);
        Task CreateRole(Role role);
    }

    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;

        public UserRepository(MongoDBContext context)
        {
            _context = context;
        }

        public async Task CreateUser(Users user)
        {
            await _context.Users.InsertOneAsync(user);
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserById(Guid userId)
        {
            return await _context.Users.Find(u => u.UserID == userId).FirstOrDefaultAsync();
        }

        public async Task UpdateUser(Guid userId, Users updatedUser)
        {
            await _context.Users.ReplaceOneAsync(u => u.UserID == userId, updatedUser);
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

        public async Task CreateRole(Role role)
        {
            await _context.Roles.InsertOneAsync(role);
        }
    }
}
