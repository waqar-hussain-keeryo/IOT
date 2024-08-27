using IOT.Entities.Models;
using IOT.Entities.Request;
using MongoDB.Driver;

namespace IOT.Data.Repositories
{
    public interface IUserRepository
    {
        Task<Users> CreateUser(Users user);
        Task<Users> UpdateUser(Guid userId, Users request);
        Task<bool> DeleteUser(Guid userId);

        Task<Role> CreateRole(Role role);
        Task<Role> UpdateRole(Guid roleId, Role updatedRole);
        Task<bool> DeleteRole(Guid roleId);
        Task<Role> GetRoleById(Guid roleId);
        Task<Role> GetRoleByName(string roleName);
        Task<(IEnumerable<Role> Roles, int TotalRecords)> GetAllRole(PaginationRequest request);

        Task<bool> VerifyUser(Guid userId);
        Task<Users> GetUserById(Guid userId);
        Task<Users> GetUserByEmail(string email);
        Task<Users> GetUserByRoleId(Guid roleId);
        Task<Users> GetUserByCustomerId(Guid customerId);
        Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsers(PaginationRequest request);
        Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByCustomerId(Guid customerId, PaginationRequest request);
        Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByRoleId(Guid? roleId, PaginationRequest request);
    }

    public class UserRepository : IUserRepository
    {
        private readonly MongoDBContext _context;

        public UserRepository(MongoDBContext context)
        {
            _context = context;
        }

        // User Methods
        public async Task<Users> CreateUser(Users user)
        {
            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<Users> UpdateUser(Guid userId, Users request)
        {
            var existingUser = await _context.Users.Find(u => u.UserID == userId).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return null;
            }

            var updateUser = Builders<Users>.Update.Combine();

            if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
                updateUser = updateUser.Set(u => u.Email, request.Email);

            if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != existingUser.FirstName)
                updateUser = updateUser.Set(u => u.FirstName, request.FirstName);

            if (!string.IsNullOrEmpty(request.LastName) && request.LastName != existingUser.LastName)
                updateUser = updateUser.Set(u => u.LastName, request.LastName);

            if (request.RoleID != Guid.Empty && request.RoleID != existingUser.RoleID)
                updateUser = updateUser.Set(u => u.RoleID, request.RoleID);

            if (request.CustomerID.HasValue && request.CustomerID != existingUser.CustomerID)
                updateUser = updateUser.Set(u => u.CustomerID, request.CustomerID);

            if (!string.IsNullOrEmpty(request.Password))
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                updateUser = updateUser.Set(u => u.Password, hashedPassword);
            }

            if (request.EmailVerified != existingUser.EmailVerified)
                updateUser = updateUser.Set(u => u.EmailVerified, request.EmailVerified);

            if (request.IsDeleted != existingUser.IsDeleted)
                updateUser = updateUser.Set(u => u.IsDeleted, request.IsDeleted);

            if (updateUser != Builders<Users>.Update.Combine())
            {
                await _context.Users.UpdateOneAsync(u => u.UserID == userId, updateUser);
            }

            return existingUser;
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            var updateUser = Builders<Users>.Update.Set(u => u.IsDeleted, true);
            var updateResult = await _context.Users.UpdateOneAsync(u => u.UserID == userId,updateUser);

            return updateResult.ModifiedCount > 0;
        }


        // Roles Methods
        public async Task<Role> CreateRole(Role role)
        {
            if (role.RoleID == Guid.Empty)
            {
                role.RoleID = Guid.NewGuid();
            }

            await _context.Roles.InsertOneAsync(role);

            return role;
        }

        public async Task<Role> UpdateRole(Guid roleId, Role request)
        {
            var existingRole = await _context.Roles.Find(r => r.RoleID == roleId).FirstOrDefaultAsync();
            if (existingRole == null)
            {
                return null;
            }

            var updateDefinitions = new List<UpdateDefinition<Role>>();

            if (!string.IsNullOrEmpty(request.RoleName) && request.RoleName != existingRole.RoleName)
            {
                updateDefinitions.Add(Builders<Role>.Update.Set(u => u.RoleName, request.RoleName));
            }

            if (updateDefinitions.Count > 0)
            {
                var updateRole = Builders<Role>.Update.Combine(updateDefinitions);
                await _context.Roles.UpdateOneAsync(r => r.RoleID == roleId, updateRole);
            }

            return existingRole;
        }

        public async Task<bool> DeleteRole(Guid roleId)
        {
            var updateRole = Builders<Role>.Update.Set(r => r.IsDeleted, true);
            var updateResult = await _context.Roles.UpdateOneAsync(r => r.RoleID == roleId, updateRole);

            return updateResult.ModifiedCount > 0;
        }

        public async Task<Role> GetRoleById(Guid roleId)
        {
            return await _context.Roles.Find(r => r.RoleID == roleId).FirstOrDefaultAsync();
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return await _context.Roles.Find(r => r.RoleName == roleName).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Role> Roles, int TotalRecords)> GetAllRole(PaginationRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var collection = _context.Roles;
            var totalRecords = await collection.CountDocumentsAsync(_ => true);

            var roles = await collection.Find(_ => true)
                                  .Skip((request.PageNumber - 1) * request.PageSize)
                                  .Limit(request.PageSize)
                                  .ToListAsync();

            return (roles, (int)totalRecords);
        }


        // User Get Methods
        public async Task<bool> VerifyUser(Guid userId)
        {
            var updateUser = Builders<Users>.Update.Set(u => u.EmailVerified, true);
            var updateResult = await _context.Users.UpdateOneAsync(u => u.UserID == userId, updateUser);

            return updateResult.ModifiedCount > 0;
        }

        public async Task<Users> GetUserById(Guid userId)
        {
            return await _context.Users.Find(u => u.UserID == userId && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _context.Users.Find(u => u.Email == email && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByRoleId(Guid roleId)
        {
            return await _context.Users.Find(u => u.RoleID == roleId && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByCustomerId(Guid customerId)
        {
            return await _context.Users.Find(u => u.CustomerID == customerId).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsers(PaginationRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var collection = _context.Users;
            var totalRecords = await collection.CountDocumentsAsync(_ => true);

            var users = await collection.Find(_ => true)
                                  .Skip((request.PageNumber - 1) * request.PageSize)
                                  .Limit(request.PageSize)
                                  .ToListAsync();

            return (users, (int)totalRecords);
        }

        public async Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByCustomerId(Guid customerId, PaginationRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var collection = _context.Users;
            var totalRecords = await collection.CountDocumentsAsync(u => u.CustomerID == customerId && u.IsDeleted == false);

            var users = await collection.Find(_ => true)
                                  .Skip((request.PageNumber - 1) * request.PageSize)
                                  .Limit(request.PageSize)
                                  .ToListAsync();

            return (users, (int)totalRecords);
        }

        public async Task<(IEnumerable<Users> Users, int TotalRecords)> GetAllUsersByRoleId(Guid? roleId, PaginationRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var collection = _context.Users;
            var totalRecords = await collection.CountDocumentsAsync(u => u.RoleID == roleId && u.IsDeleted == false);

            var users = await collection.Find(_ => true)
                                  .Skip((request.PageNumber - 1) * request.PageSize)
                                  .Limit(request.PageSize)
                                  .ToListAsync();

            return (users, (int)totalRecords);
        }
    }
}
