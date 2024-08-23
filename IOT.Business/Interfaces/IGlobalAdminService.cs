using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<ResponseDTO> RegisterGlobalAdmin(UserDTO admin);
    }
}
