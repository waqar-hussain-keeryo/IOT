using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;

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

        public async Task<ResponseDTO> Login(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                var role = await _userRepository.GetRoleById(user.RoleID);
                var token = _jwtTokenGenerator.GenerateToken(user, role.RoleName);

                var loginResponse = new LoginResponseDTO
                {
                    Email = user.Email,
                    Token = token,
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = loginResponse
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> RegisterUser(UserDTO user, string customerId)
        {
            try
            {
                if (user.RoleName == "Admin")
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Admin role cannot be registered by this method. Please contact your administrator."
                    };
                }

                var existingUser = await _userRepository.GetUserByEmail(user.Email);
                if (existingUser != null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "User already registered."
                    };
                }

                var role = await _userRepository.GetRoleByName(user.RoleName);
                if (role == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role does not exist."
                    };
                }

                // Convert customerId from string to Guid
                Guid? customerGuid = null;
                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    if (Guid.TryParse(customerId, out var parsedGuid))
                    {
                        customerGuid = parsedGuid;
                    }
                    else
                    {
                        return new ResponseDTO
                        {
                            Success = false,
                            Message = "Invalid customer ID format."
                        };
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

                await _userRepository.CreateUser(newUser);

                var token = _jwtTokenGenerator.GenerateToken(newUser, user.RoleName);

                var loginResponse = new LoginResponseDTO
                {
                    Email = user.Email,
                    Token = token,
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "User registered successfully.",
                    Data = loginResponse
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetAllUsers()
        {
            try
            {
                // Retrieve all users from the repository
                var users = await _userRepository.GetAllUsers();

                // Fetch role names and map users to UserDTOs
                var userDTOs = users.Select(async u =>
                {
                    var role = await _userRepository.GetRoleById(u.RoleID);
                    return new UserDTO
                    {
                        UserID = u.UserID,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        RoleName = role.RoleName
                    };
                }).ToList();

                // Wait for all userDTO tasks to complete
                var userDTOList = await Task.WhenAll(userDTOs);

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = userDTOList
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Page number must be greater than zero."
                };
            }

            if (pageSize < 1)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "Page size must be greater than zero."
                };
            }

            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Customer ID cannot be null or empty."
                    };
                }

                // Parse customerId to Guid
                if (!Guid.TryParse(customerId, out var customerGuid))
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid customer ID format."
                    };
                }

                // Retrieve paginated users by customer ID
                var (users, totalRecords) = await _userRepository.GetAllUsersByCustomerId(customerGuid, pageNumber, pageSize);

                // Map to UserDTO
                var userDTOs = users.Select(u => new UserDTO
                {
                    UserID = u.UserID,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                }).ToList();

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = new
                    {
                        Users = userDTOs,
                        TotalRecords = totalRecords,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetUserById(Guid userId)
        {
            try
            {
                // Retrieve user by ID
                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Retrieve user's role
                var role = await _userRepository.GetRoleById(user.RoleID);
                if (role == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "User's role not found."
                    };
                }

                // Map user and role to UserDTO
                var userDTO = new UserDTO
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = role.RoleName
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = userDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetUserByEmail(string email, string userId)
        {
            try
            {
                // Retrieve user by email
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Parse userId to Guid
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid user ID format."
                    };
                }

                // Check if the provided userId matches the user's ID
                if (user.UserID != userGuid)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "You are not authorized to view this user's details."
                    };
                }

                // Retrieve user's role
                var role = await _userRepository.GetRoleById(user.RoleID);
                if (role == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role not found for the user."
                    };
                }

                // Map user and role to UserDTO
                var userDTO = new UserDTO
                {
                    UserID = user.UserID,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = role.RoleName
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = userDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }


        public async Task<ResponseDTO> CreateRole(RoleDTO roleDTO)
        {
            try
            {
                // Check if the role already exists
                var existingRole = await _userRepository.GetRoleByName(roleDTO.RoleName);
                if (existingRole != null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role already exists."
                    };
                }

                // Create the new role
                var newRole = new Role
                {
                    RoleName = roleDTO.RoleName,
                    RoleDescription = roleDTO.RoleDescription
                };

                await _userRepository.CreateRole(newRole);

                // Prepare the response with the created role information
                var createdRoleDTO = new RoleDTO
                {
                    RoleID = newRole.RoleID,
                    RoleName = newRole.RoleName,
                    RoleDescription = newRole.RoleDescription
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Role successfully created.",
                    Data = createdRoleDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetRoleByName(string roleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role name cannot be null or empty."
                    };
                }

                var existingRole = await _userRepository.GetRoleByName(roleName);

                if (existingRole == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role not found."
                    };
                }

                var roleDTO = new RoleDTO
                {
                    RoleID = existingRole.RoleID,
                    RoleName = existingRole.RoleName,
                    RoleDescription = existingRole.RoleDescription
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Role retrieved successfully.",
                    Data = roleDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

        public async Task<ResponseDTO> GetRoleById(Guid roleId)
        {
            try
            {
                if (roleId == Guid.Empty)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Invalid role ID."
                    };
                }

                var role = await _userRepository.GetRoleById(roleId);

                if (role == null)
                {
                    return new ResponseDTO
                    {
                        Success = false,
                        Message = "Role not found."
                    };
                }

                var roleDTO = new RoleDTO
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName,
                    RoleDescription = role.RoleDescription
                };

                return new ResponseDTO
                {
                    Success = true,
                    Message = "Role retrieved successfully.",
                    Data = roleDTO
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                };
            }
        }

    }
}
