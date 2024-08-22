using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Services
{
    public class GlobalAdminService : IGlobalAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public GlobalAdminService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> RegisterGlobalAdmin(UserDTO user)
        {
            // Ensure the role exists or not
            var globalAdminRoleId = Guid.NewGuid();
            var role = await _userRepository.GetRoleByName(user.RoleName);

            if (role == null)
            {
                // Create the global admin role if it does not exist
                var newRole = new Role
                {
                    RoleName = "Admin",
                    RoleDescription = "Global Administrator role"
                };

                await _userRepository.CreateRole(newRole);
                globalAdminRoleId = role.RoleID;
            }
            else
            {
                globalAdminRoleId = role.RoleID;
            }

            // Create the global admin
            var admin = new Users
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                RoleID = globalAdminRoleId
            };

            await _userRepository.CreateUser(admin);
            var token = _jwtTokenGenerator.GenerateToken(admin, user.RoleName);
            return token;
        }
    }
}
