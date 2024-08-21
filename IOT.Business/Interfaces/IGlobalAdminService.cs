using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Interfaces
{
    public interface IGlobalAdminService
    {
        Task<IEnumerable<UserDTO>> GetAllAdmins();
        Task<UserDTO> GetAdminById(int id);
        Task CreateAdmin(UserDTO adminDto);
        Task UpdateAdmin(UserDTO adminDto);
        Task DeleteAdminById(int id);
    }
}
