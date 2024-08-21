using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using Microsoft.Extensions.Configuration;

namespace IOT.Business.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public RoleService(IRoleRepository roleRepository, IConfiguration configuration)
        {
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<string> CreateRole(RoleDTO role)
        {
            try
            {
                var existingRole = await _roleRepository.GetRoleByName(role.RoleName);

                if (existingRole == null)
                {
                    var newRole = new Role
                    {
                        RoleName = role.RoleName,
                        RoleDescription = role.RoleDescription
                    };

                    await _roleRepository.CreateRole(newRole);
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
                var existingRole = await _roleRepository.GetRoleByName(roleName);
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
            var role = await _roleRepository.GetRoleById(roleId);
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
