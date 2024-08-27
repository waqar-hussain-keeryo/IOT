using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<ResponseDTO> CreateAdmin(UserRequest user);
        Task<ResponseDTO> UpdateAdmin(string email, string roleName, UserRequest request);
        Task<ResponseDTO> DeleteAdmin(string email, string roleName);
        Task<ResponseDTO> GetAllAdmin(string roleName, PaginationRequest request);
        Task<ResponseDTO> GetAllUsers(string roleName, PaginationRequest request);
        Task<ResponseDTO> GetAllCustomers(string roleName, PaginationRequest request);

        Task<ResponseDTO> CreateCustomer(string roleName, UserRequest user);
        Task<ResponseDTO> UpdateCustomer(string email, string roleName, UserRequest request);
        Task<ResponseDTO> DeleteCustomer(string email, string roleName);

        Task<ResponseDTO> CreateRole(string roleName, RoleRequest request);
        Task<ResponseDTO> UpdateRole(string roleName, string updateRoleName, RoleRequest request);
        Task<ResponseDTO> DeleteRole(string currentRoleName, string deletedRoleName);
    }
}
