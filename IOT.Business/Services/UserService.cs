using IOT.Business.Interfaces;
using IOT.Entities.DTO;

namespace IOT.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user.ToDto();
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => u.ToDto());
        }

        public async Task CreateUserAsync(UserDTO UserDTO)
        {
            var user = UserDTO.ToEntity();
            await _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(UserDTO UserDTO)
        {
            var user = UserDTO.ToEntity();
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}
