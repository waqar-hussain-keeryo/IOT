using IOT.Business.Validations;
using IOT.Entities.DTO;

namespace IOT.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(int id);
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task CreateUser(UserDTO userDto);
        Task UpdateUser(UserDTO userDto);
        Task DeleteUserById(int id);
    }
}
