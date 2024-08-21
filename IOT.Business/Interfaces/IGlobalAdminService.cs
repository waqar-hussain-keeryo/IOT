using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<string> RegisterUser(UserDTO user);
        Task<string> RegisterGlobalAdmin(UserDTO admin);
        Task<string> Login(string email, string password);
    }
}
