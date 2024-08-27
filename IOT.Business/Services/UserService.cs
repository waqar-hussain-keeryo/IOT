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

        public async Task<ResponseDTO> CreateUser(string userId, string roleName, UserRequest request)
        {
            try
            {
                if (roleName == "User" || request.RoleName != "User")
                {
                    return new ResponseDTO(false, "You are not authorized to register account. Please contact your administrator.");
                }

                var existingUser = await _userRepository.GetUserByEmail(request.Email);
                if (existingUser != null)
                {
                    return new ResponseDTO(false, "User already registered.");
                }

                var role = await _userRepository.GetRoleByName(request.RoleName);
                if (role == null)
                {
                    return new ResponseDTO(false, "Role does not exist.");
                }

                var customerId = CommonMethods.ValidateGuid(userId);

                // Create the new user
                var newUser = new Users
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    RoleID = role.RoleID,
                    CustomerID = roleName == "Customer" ? customerId : null,
                };

                var userResponse = await _userRepository.CreateUser(newUser);
                var token = _jwtTokenGenerator.GenerateToken(newUser, request.RoleName);
                var responseDTO = new UserDTO(userResponse);
                responseDTO.Token = token;
                
                return new ResponseDTO(true, "User successfully registered.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateUser(Guid userId, string roleName, UserRequest request)
        {
            try
            {
                if (roleName == "User" || request.RoleName != "User")
                {
                    return new ResponseDTO(false, "You are not authorized to register account. Please contact your administrator.");
                }

                var currentUser = await _userRepository.GetUserById(userId);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "User not found.");
                }

                var roleId = await _userRepository.GetRoleByName(request.RoleName);

                currentUser.FirstName = request.FirstName;
                currentUser.LastName = request.LastName;
                currentUser.Email = request.Email;
                currentUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                currentUser.RoleID = roleId.RoleID;

                var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                var responseDTO = new UserDTO(updateResponse);
                return new ResponseDTO(true, "User successfully updated.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteUser(Guid userId, string roleName)
        {
            try
            {
                if (roleName == "User")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var currentUser = await _userRepository.GetUserById(userId);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "User not found.");
                }

                currentUser.IsDeleted = true;

                var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                var responseDTO = new UserDTO(updateResponse);
                return new ResponseDTO(true, "User successfully deleted.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> VerifyUser(Guid userId, string roleName)
        {
            try
            {
                if (roleName == "User")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                var currentUser = await _userRepository.GetUserById(userId);
                if (currentUser == null)
                {
                    return new ResponseDTO(false, "User not found.");
                }

                currentUser.EmailVerified = true;

                var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                var responseDTO = new UserDTO(updateResponse);
                return new ResponseDTO(true, "User successfully verified.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

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
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    RoleName = role.RoleName,
                    EmailVerified = user.EmailVerified
                };

                return new ResponseDTO(true, "User retrieved successfully.", userDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, string roleName, PaginationRequest request)
        {
            try
            {
                if (roleName == "User")
                {
                    return new ResponseDTO(false, "You are not authorized.");
                }

                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return new ResponseDTO(false, "Customer ID cannot be null or empty.");
                }

                // Parse customerId to Guid
                var customerGuid = CommonMethods.ValidateGuid(customerId);
                var (users, totalRecords) = await _userRepository.GetAllUsersByCustomerId(customerGuid, request);

                var userDTOs = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Password = u.Password,
                    RoleID = u.RoleID,
                    CustomerID = customerGuid,
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
    }
}
