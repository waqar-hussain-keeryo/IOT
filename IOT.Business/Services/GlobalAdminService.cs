using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.Common;
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

        public async Task<ResponseDTO> RegisterAdmin(UserRequest user)
        {
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null)
            {
                return new ResponseDTO(false, "Admin already registered.");
            }

            if(user.RoleName != "Admin")
            {
                return new ResponseDTO(false, "Only register admin account.");
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

            return new ResponseDTO(true, "Global Admin Registered successfully", responseDTO);
        }

        public async Task<ResponseDTO> RegisterCustomer(UserRequest user)
        {
            try
            {
                if (user.RoleName == "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized to register admin. Please contact your administrator.");
                }

                if (user.RoleName != "Customer")
                {
                    return new ResponseDTO(false, "Register only customer account.");
                }

                var existingUser = await _userRepository.GetUserByEmail(user.Email);
                if (existingUser != null)
                {
                    return new ResponseDTO(false, "User already registered.");
                }

                var role = await _userRepository.GetRoleByName(user.RoleName);
                if (role == null)
                {
                    return new ResponseDTO(false, "Role does not exist.");
                }

                // Create the new user
                var newUser = new Users
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    RoleID = role.RoleID,
                    CustomerID = null,
                };

                var userResponse = await _userRepository.CreateUser(newUser);

                var responseData = new
                {
                    RegisteredCustomer = userResponse,
                    RegisteredDemoUser = new Users()
                };

                // Add demo user if a customer was registered
                if (user.RoleName == "Customer")
                {
                    var userRole = await _userRepository.GetRoleByName("User");
                    if (userRole != null)
                    {
                        var demoUser = new Users
                        {
                            Email = $"demoUser_{user.Email}",
                            FirstName = "Demo",
                            LastName = "User",
                            Password = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
                            RoleID = userRole.RoleID,
                            CustomerID = userResponse.UserID
                        };

                        var response = await _userRepository.CreateUser(demoUser);

                        responseData = new
                        {
                            RegisteredCustomer = userResponse,
                            RegisteredDemoUser = response
                        };
                    }
                    else
                    {
                        return new ResponseDTO(false, "Role does not exist.");
                    }
                }

                return new ResponseDTO(true, "User registered successfully." + (responseData.RegisteredDemoUser != null ? " Demo user created successfully." : string.Empty), responseData);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
