using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

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

        public async Task<ResponseDTO> RegisterGlobalAdmin(UserRequest user)
        {
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return new ResponseDTO(false, "Admin already registered.");
            }

            // Check if the role exists
            var role = await _userRepository.GetRoleByName(user.RoleName);
            var globalAdminRoleId = role?.RoleID ?? Guid.NewGuid();

            // Check if current admin exists
            var currentAdmin = await _userRepository.GetUserByRoleId(globalAdminRoleId);
            if (currentAdmin != null)
            {
                return new ResponseDTO(false, "Only one global admin allowed.");
            }

            // Create role if it doesn't exist
            if (role == null)
            {
                var newRole = new Role
                {
                    RoleName = user.RoleName,
                    RoleDescription = "Global Administrator role"
                };

                var roleResponse = await _userRepository.CreateRole(newRole);
                globalAdminRoleId = roleResponse.RoleID;
            }

            // Create the global admin user
            var admin = new Users
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                RoleID = globalAdminRoleId
            };

            var userResponse = await _userRepository.CreateUser(admin);

            var responseDTO = new UserDTO(userResponse);

            // Generate and secure the token
            var token = _jwtTokenGenerator.GenerateToken(admin, "Admin");
            responseDTO.Token = token;

            return new ResponseDTO
            {
                Success = true,
                Message = "Global admin registered successfully.",
                Data = responseDTO
            };
        }
    }
}
