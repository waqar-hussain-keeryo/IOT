using FluentValidation;
using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IOT.Business.Services
{
    public class GlobalAdminService : IGlobalAdminService
    {
        private readonly IGlobalAdminRepository _globalAdminRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public GlobalAdminService(IGlobalAdminRepository globalAdminRepository, IRoleService roleService, IRoleRepository roleRepository, IConfiguration configuration)
        {
            _globalAdminRepository = globalAdminRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<string> RegisterGlobalAdmin(UserDTO user)
        {
            //var existingAdmin = await GetUserByEmail(user.Email);
            //if (existingAdmin != null)
            //{
            //    throw new InvalidOperationException("Admin already exists.");
            //}

            // Ensure the role exists or not
            var globalAdminRoleId = Guid.NewGuid();
            var role = await _roleRepository.GetRoleByName(user.RoleName);

            if (role == null)
            {
                // Create the global admin role if it does not exist
                var newRole = new Role
                {
                    RoleName = "Admin",
                    RoleDescription = "Global Administrator role"
                };

                await _roleRepository.CreateRole(newRole);
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

            await _globalAdminRepository.CreateUser(admin);
            var token = await GenerateJwtToken(admin);
            return token;
        }

        public async Task<string> RegisterUser(UserDTO user)
        {
            //// Check if the user already exists
            //var existingUser = await GetUserByEmail(user.Email);
            //if (existingUser != null)
            //{
            //    throw new Exception("User already exists.");
            //}

            if(user.RoleName == "Admin")
                throw new InvalidOperationException("Please contact your administrator.");

            var role = await _roleRepository.GetRoleByName(user.RoleName);
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

            await _globalAdminRepository.CreateUser(newUser);

            var token = await GenerateJwtToken(newUser);
            return token;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _globalAdminRepository.GetUserByEmail(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Retrieve role information
            var role = await _roleRepository.GetRoleById(user.RoleID);
            var roleName = role?.RoleName ?? "";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signIn
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }



        // Helper Method for JWT token
        #region JWT token private method
        private async Task<string> GenerateJwtToken(Users user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

                // Retrieve role information
                var role = await _roleRepository.GetRoleById(user.RoleID);
                var roleName = role?.RoleName ?? "";

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, roleName)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"]
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating JWT token: {ex.Message}");
                throw;
            }
        }

        #endregion
    }
}
