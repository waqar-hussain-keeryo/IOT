using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Interfaces
{
    public interface IRoleService
    {
        Task<string> CreateRole(RoleDTO role);
        Task<RoleDTO> GetRoleByName(string roleName);
        Task<RoleDTO> GetRoleById(Guid roleId);
    }
}
