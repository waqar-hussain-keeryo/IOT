using IOT.Entities.DTO;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDTO> Login(LoginRequest request);
        Task<ResponseDTO> CreateUser(string userId, string roleName, UserRequest request);
        Task<ResponseDTO> UpdateUser(Guid userId, string roleName, UserRequest request);
        Task<ResponseDTO> DeleteUser(Guid userId, string roleName);
        Task<ResponseDTO> VerifyUser(Guid userId, string roleName);
        Task<ResponseDTO> GetUserByEmail(string email);
        Task<ResponseDTO> GetAllUsersByCustomerId(string customerId, string roleName, PaginationRequest request);
    }
}
