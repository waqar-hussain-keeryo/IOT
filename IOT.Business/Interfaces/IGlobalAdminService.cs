using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<string> RegisterGlobalAdmin(UserDTO admin);
    }
}
