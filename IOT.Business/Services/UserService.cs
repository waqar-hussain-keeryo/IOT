using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IOT.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public UserService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> RegisterUser(UserDTO user)
        {
            if (user.RoleName == "Admin")
                throw new InvalidOperationException("Please contact your administrator.");

            var role = await _userRepository.GetRoleByName(user.RoleName);
            if (role == null)
            {
                throw new InvalidOperationException("Role not exist.");
            }

            // Create the new user
            var newUser = new Users
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                RoleID = role.RoleID
            };

            await _userRepository.CreateUser(newUser);

            var token = _jwtTokenGenerator.GenerateToken(newUser, user.RoleName);
            return token;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            var role = await _userRepository.GetRoleById(user.RoleID);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = _jwtTokenGenerator.GenerateToken(user, role.RoleName);
            return token;
        }

        public async Task<string> CreateRole(RoleDTO role)
        {
            try
            {
                var existingRole = await _userRepository.GetRoleByName(role.RoleName);

                if (existingRole == null)
                {
                    var newRole = new Role
                    {
                        RoleName = role.RoleName,
                        RoleDescription = role.RoleDescription
                    };

                    await _userRepository.CreateRole(newRole);
                    return "Role Successfully created";
                }
                else
                {
                    throw new InvalidOperationException("Role already exists.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RoleDTO> GetRoleByName(string roleName)
        {
            try
            {
                var existingRole = await _userRepository.GetRoleByName(roleName);
                RoleDTO role = null;

                if (existingRole != null)
                {
                    role = new RoleDTO
                    {
                        RoleID = existingRole.RoleID,
                        RoleName = existingRole.RoleName,
                        RoleDescription = existingRole.RoleDescription
                    };

                    return role;
                }
                else
                {
                    throw new Exception("Role not found.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RoleDTO> GetRoleById(Guid roleId)
        {
            var role = await _userRepository.GetRoleById(roleId);
            if (role == null) return null;

            return new RoleDTO
            {
                RoleID = role.RoleID,
                RoleName = role.RoleName,
                RoleDescription = role.RoleDescription
            };
        }
    }
}
