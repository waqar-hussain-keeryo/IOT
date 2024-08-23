using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.Common;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

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

        //Access All Users
        public async Task<ResponseDTO> Login(LoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(request.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return new ResponseDTO(false, "Invalid email or password.");
                }

                var role = await _userRepository.GetRoleById(user.RoleID);
                var token = _jwtTokenGenerator.GenerateToken(user, role.RoleName);
                var loginResponse = new LoginResponseDTO(user.Email, role.RoleName, token);

                return new ResponseDTO(true, "Login Successfull.", loginResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access All Users
        public async Task<ResponseDTO> RegisterUser(UserRequest user, string customerId)
        {
            try
            {
                if (user.RoleName == "Admin")
                {
                    return new ResponseDTO(false, "Admin role cannot be registered by this method. Please contact your administrator.");
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

                // Convert customerId from string to Guid
                Guid? customerGuid = null;
                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    var parsedGuid = CommonMethods.ValidateGuid(customerId);
                    if (customerGuid == Guid.Empty)
                    {
                        return new ResponseDTO(false, "Invalid user ID format.");
                    }
                    else
                    {
                        customerGuid = parsedGuid;
                    }
                }

                // Create the new user
                var newUser = new Users
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    RoleID = role.RoleID,
                    CustomerID = user.RoleName == "User" ? customerGuid : null,
                };

                var userResponse = await _userRepository.CreateUser(newUser);
                var token = _jwtTokenGenerator.GenerateToken(newUser, user.RoleName);
                var loginResponse = new LoginResponseDTO(user.Email, role.RoleName, token);

                var responseData = new
                {
                    RegisteredUser = loginResponse,
                    DemoUser = ""
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
                            RegisteredUser = loginResponse,
                            DemoUser = response.Email
                        };
                    }  
                }

                return new ResponseDTO(true, "User registered successfully." + (responseData.DemoUser != null ? " Demo user created successfully." : string.Empty), responseData);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access All Users
        public async Task<ResponseDTO> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return new ResponseDTO(false, "User not found.");
                }

                var role = await _userRepository.GetRoleById(user.RoleID);
                var userDTO = new UserDTO
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = role.RoleName
                };

                return new ResponseDTO(true, "User retrieved successfully.", userDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access Only Customer
        public async Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, PaginationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return new ResponseDTO(false, "Customer ID cannot be null or empty.");
                }

                // Parse customerId to Guid
                var customerGuid = CommonMethods.ValidateGuid(customerId);
                if (customerGuid == Guid.Empty)
                {
                    return new ResponseDTO(false, "Invalid user ID format.");
                }

                var (users, totalRecords) = await _userRepository.GetAllUsersByCustomerId(customerGuid, request);

                var userDTOs = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleID = u.RoleID,
                    CustomerID = customerGuid,
                    Password = u.Password,
                    EmailVerified = u.EmailVerified
                }).ToList();

                return new ResponseDTO(true, "Users retrieved successfully.",
                    new
                    {
                        Users = userDTOs,
                        TotalRecords = totalRecords,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access Only Admin
        public async Task<ResponseDTO> GetAllUsers(PaginationRequest request)
        {
            try
            {
                var (users, totalRecords) = await _userRepository.GetAllUsers(request);
                var userDTOs = users.Select(async u =>
                {
                    var role = await _userRepository.GetRoleById(u.RoleID);
                    return new UserDTO
                    {
                        UserID = u.UserID,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        CustomerID = u.CustomerID,
                        Password = u.Password,
                        EmailVerified = u.EmailVerified
                    };
                }).ToList();

                // Wait for all userDTO tasks to complete
                var userList = await Task.WhenAll(userDTOs);

                return new ResponseDTO(true, "Users retrieved successfully.",
                    new
                    {
                        Users = userList,
                        TotalRecords = totalRecords,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access Only Admin
        public async Task<ResponseDTO> CreateRole(RoleRequest role)
        {
            try
            {
                var existingRole = await _userRepository.GetRoleByName(role.RoleName);
                if (existingRole != null)
                {
                    return new ResponseDTO(false, "Role already exists.");
                }

                // Create the new role
                var newRole = new Role
                {
                    RoleName = role.RoleName,
                    RoleDescription = role.RoleDescription
                };

                var response = await _userRepository.CreateRole(newRole);
                var roleDTO = new RoleDTO(response);

                return new ResponseDTO(true, "Role successfully created.", roleDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        //Access Only Admin
        public async Task<ResponseDTO> UpdateRole(RoleRequest role)
        {
            try
            {
                var existingRole = await _userRepository.GetRoleByName(role.RoleName);
                if (existingRole == null)
                {
                    return new ResponseDTO(false, "Role not found.");
                }

                existingRole.RoleName = role.RoleName;
                existingRole.RoleDescription = role.RoleDescription;

                var updatedRole = await _userRepository.UpdateRole(existingRole.RoleID, existingRole);

                if (updatedRole != null)
                {
                    var roleDTO = new RoleDTO(updatedRole);
                    return new ResponseDTO(true, "Role successfully updated.", updatedRole);
                }
                else
                {
                    return new ResponseDTO(false, "Failed to update the role.");
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

    }
}
