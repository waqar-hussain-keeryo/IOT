using IOT.Entities.DTO;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> Login(LoginRequest request);
        Task<ResponseDTO> RegisterUser(UserRequest user, string userId);
        Task<ResponseDTO> GetAllUsers(PaginationRequest request);
        Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, PaginationRequest request);
        Task<ResponseDTO> GetUserByEmail(string email);
        Task<ResponseDTO> CreateRole(RoleRequest role);
        Task<ResponseDTO> UpdateRole(RoleRequest role);
        //Task<ResponseDTO> DeleteRole(RoleDTO role);
    }
}
