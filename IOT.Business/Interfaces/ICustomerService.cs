using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseDTO> CreateCustomer(string roleName, CustomerRequest request);
        Task<ResponseDTO> UpdateCustomer(string roleName, Guid customerId, CustomerRequest request);
        Task<ResponseDTO> DeleteCustomer(string roleName, Guid customerId);
        Task<ResponseDTO> GetCustomerById(string roleName, Guid customerId);
        Task<ResponseDTO> GetAllCustomers(string roleName, PaginationRequest request);


        Task<ResponseDTO> CreateSites(string roleName, Guid customerId, SiteRequest request);
        Task<ResponseDTO> UpdateSites(string roleName, Guid customerId, SiteRequest request);

        Task<ResponseDTO> AddDevices(string roleName, Guid customerId, DeviceRequest request);
        Task<ResponseDTO> AddCustomerUsers(string roleName, Guid customerId, List<string> CustomerUsers);
        Task<ResponseDTO> CreateDigitalServices(string roleName, Guid customerId, DigitalServiceRequest request);
        Task<ResponseDTO> AddNotificationUsers(string roleName, Guid customerId, List<string> NotificationUsers);
    }
}
