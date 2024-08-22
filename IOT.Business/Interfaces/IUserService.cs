using IOT.Entities.DTO;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterUser(UserDTO user);
        Task<string> Login(string email, string password);
        Task<string> CreateRole(RoleDTO role);
        Task<RoleDTO> GetRoleByName(string roleName);
        Task<RoleDTO> GetRoleById(Guid roleId);
    }
}
