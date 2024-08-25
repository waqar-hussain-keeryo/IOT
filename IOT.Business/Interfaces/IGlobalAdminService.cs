using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<ResponseDTO> RegisterAdmin(UserRequest user);
        Task<ResponseDTO> RegisterCustomer(UserRequest user);
    }
}
