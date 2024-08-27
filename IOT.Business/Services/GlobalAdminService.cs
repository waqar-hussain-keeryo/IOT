using Amazon.Runtime.Internal;
using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.Common;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;
using System.Data;

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

        public async Task<ResponseDTO> CreateAdmin(UserRequest user)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByEmail(user.Email);
                if (existingUser != null)
                {
                    return new ResponseDTO(false, "Admin already registered.");
                }

                if (user.RoleName != "Admin")
                {
                    return new ResponseDTO(false, "Only register admin account.");
                }

                var role = await _userRepository.GetRoleByName(user.RoleName);
                var adminRoleId = role?.RoleID ?? Guid.NewGuid();

                var currentAdmin = await _userRepository.GetUserByRoleId(adminRoleId);
                if (currentAdmin != null)
                {
                    return new ResponseDTO(false, "Only one global admin allowed.");
                }

                if (role == null)
                {
                    var newRole = new Role
                    {
                        RoleName = user.RoleName,
                        RoleDescription = "Global Admin role registered."
                    };

                    var roleResponse = await _userRepository.CreateRole(newRole);
                    adminRoleId = roleResponse.RoleID;
                }

                var admin = new Users
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    RoleID = adminRoleId
                };

                var userResponse = await _userRepository.CreateUser(admin);
                var responseDTO = new UserDTO(userResponse);
                var token = _jwtTokenGenerator.GenerateToken(admin, "Admin");
                responseDTO.Token = token;

                return new ResponseDTO(true, "Global Admin Registered successfully.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateAdmin(string email, string roleName, UserRequest request)
        {
            try
            {
                var currentUser = await _userRepository.GetUserByEmail(email);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "Admin not found.");
                }

                // Authorization logic
                if (roleName == "Admin")
                {
                    var roleId = await _userRepository.GetRoleByName(request.RoleName);

                    currentUser.FirstName = request.FirstName;
                    currentUser.LastName = request.LastName;
                    currentUser.Email = request.Email;
                    currentUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    currentUser.RoleID = roleId.RoleID;

                    var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                    var responseDTO = new UserDTO(updateResponse);
                    return new ResponseDTO(true, "Admin successfully updated.", responseDTO);
                }
                else
                {
                    return new ResponseDTO(false, "Unauthorized role.");
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteAdmin(string email, string roleName)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized to delete admin account.");
                }

                var currentUser = await _userRepository.GetUserByEmail(email);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "Admin account not found.");
                }

                currentUser.IsDeleted = true;

                var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                var responseDTO = new UserDTO(updateResponse);
                return new ResponseDTO(true, "Admin successfully deleted.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAdminById(string userId, string roleName)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var userGuid = CommonMethods.ValidateGuid(userId);
                var currentUser = await _userRepository.GetUserById(userGuid);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "Admin account not found.");
                }

                var responseDTO = new UserDTO(currentUser);
                return new ResponseDTO(true, "Admin account.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllAdmin(string roleName, PaginationRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var roleId = await _userRepository.GetRoleByName(roleName);
                var (users, totalRecords) = await _userRepository.GetAllUsersByRoleId(roleId.RoleID, request);

                var userDTOs = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleID = u.RoleID,
                    CustomerID = u.CustomerID,
                    Password = u.Password,
                    EmailVerified = u.EmailVerified,
                    IsDeleted = u.IsDeleted
                }).ToList();

                return new ResponseDTO(true, "Admin retrieved successfully.",
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

        public async Task<ResponseDTO> GetAllUsers(string roleName, PaginationRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var (users, totalRecords) = await _userRepository.GetAllUsers(request);
                var userDTOs = users.Select(async u =>
                {
                    var role = await _userRepository.GetRoleById(u.RoleID);
                    return new UserDTO
                    {
                        UserID = u.UserID,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Password = u.Password,
                        RoleID = role.RoleID,
                        RoleName = role.RoleName,
                        CustomerID = u.CustomerID,
                        EmailVerified = u.EmailVerified,
                        IsDeleted = u.IsDeleted
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

        public async Task<ResponseDTO> GetAllCustomers(string roleName, PaginationRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var roleId = await _userRepository.GetRoleByName("Customer");
                if (roleId == null)
                {
                    return new ResponseDTO(false, "Role not exist.");
                }

                var (users, totalRecords) = await _userRepository.GetAllUsersByRoleId(roleId.RoleID, request);

                var userDTOs = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleID = u.RoleID,
                    CustomerID = u.CustomerID,
                    Password = u.Password,
                    EmailVerified = u.EmailVerified,
                    IsDeleted = u.IsDeleted
                }).ToList();

                return new ResponseDTO(true, "Customer retrieved successfully.",
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


        public async Task<ResponseDTO> CreateCustomer(string roleName, UserRequest user)
        {
            try
            {
                if (roleName != "Admin" && (user.RoleName == "Admin" || user.RoleName == "User"))
                {
                    return new ResponseDTO(false, "You are not authorized to register account. Please contact your administrator.");
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

                var newUser = new Users
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
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
                var userRole = await _userRepository.GetRoleByName("CustomerAdmin");
                if (userRole != null)
                {
                    var demoUser = new Users
                    {
                        Email = $"demoCustomerAdmin_{user.Email}",
                        FirstName = "Customer",
                        LastName = "Admin",
                        Password = BCrypt.Net.BCrypt.HashPassword("Customer123+"),
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

                return new ResponseDTO(true, "Customer registered successfully." + (responseData.RegisteredDemoUser != null ? " Customer admin created successfully." : string.Empty), responseData);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateCustomer(string email, string roleName, UserRequest request)
        {
            try
            {
                var currentUser = await _userRepository.GetUserByEmail(email);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                if (roleName == "Admin")
                {
                    var roleId = await _userRepository.GetRoleByName(request.RoleName);

                    currentUser.Email = request.Email;
                    currentUser.FirstName = request.FirstName;
                    currentUser.LastName = request.LastName;
                    currentUser.RoleID = roleId.RoleID;
                    currentUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                    var responseDTO = new UserDTO(updateResponse);
                    return new ResponseDTO(true, "Customer successfully updated.", responseDTO);
                }
                else
                {
                    return new ResponseDTO(false, "Unauthorized role.");
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteCustomer(string email, string roleName)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var currentUser = await _userRepository.GetUserByEmail(email);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                currentUser.IsDeleted = true;

                var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                var responseDTO = new UserDTO(updateResponse);
                return new ResponseDTO(true, "Customer successfully deleted.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }


        public async Task<ResponseDTO> CreateRole(string roleName, RoleRequest role)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

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

        public async Task<ResponseDTO> UpdateRole(string roleName, string updateRoleName, RoleRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var existingRole = await _userRepository.GetRoleByName(updateRoleName);
                if (existingRole == null)
                {
                    return new ResponseDTO(false, "Role not found.");
                }

                existingRole.RoleName = request.RoleName;
                existingRole.RoleDescription = request.RoleDescription;

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

        public async Task<ResponseDTO> DeleteRole(string currentRoleName, string deletedRoleName)
        {
            try
            {
                if (currentRoleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var currentRole = await _userRepository.GetRoleByName(deletedRoleName);
                if (currentRole == null)
                {
                    return new ResponseDTO(false, "Role not found.");
                }

                currentRole.IsDeleted = true;

                var updateResponse = await _userRepository.DeleteRole(currentRole.RoleID);
                return new ResponseDTO(true, "Role successfully deleted.", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
