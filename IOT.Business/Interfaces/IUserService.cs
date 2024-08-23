using IOT.Entities.DTO;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> Login(string email, string password);
        Task<ResponseDTO> RegisterUser(UserDTO user, string userId);
        Task<ResponseDTO> GetAllUsers();
        Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, int pageNumber = 1, int pageSize = 10);
        Task<ResponseDTO> GetUserById(Guid userId);
        Task<ResponseDTO> GetUserByEmail(string email, string userId);
        Task<ResponseDTO> CreateRole(RoleDTO role);
        Task<ResponseDTO> GetRoleByName(string roleName);
        Task<ResponseDTO> GetRoleById(Guid roleId);
    }
}
