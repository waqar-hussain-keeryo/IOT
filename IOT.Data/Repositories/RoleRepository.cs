using IOT.Entities.Models;
using MongoDB.Driver;

namespace IOT.Data.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByName(string roleName);
        Task<Role> GetRoleById(Guid roleId);
        Task CreateRole(Role role);
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly MongoDBContext _context;
        public RoleRepository(MongoDBContext context)
        {
            _context = context;
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
