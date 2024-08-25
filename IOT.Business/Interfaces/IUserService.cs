using IOT.Entities.DTO;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> Login(LoginRequest request);
        Task<ResponseDTO> RegisterUser(UserRequest request, string currentRole, string userId);
        Task<ResponseDTO> UpdateUser(string email, UserRequest request, string roleName);
        Task<ResponseDTO> GetAllUsers(PaginationRequest request);
        Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, PaginationRequest request);
        Task<ResponseDTO> GetUserByEmail(string email);
        Task<ResponseDTO> CreateRole(RoleRequest role);
        Task<ResponseDTO> UpdateRole(string roleName, RoleRequest role);
    }
}
