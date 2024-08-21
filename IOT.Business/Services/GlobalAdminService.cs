using FluentValidation;
using IOT.Business.Interfaces;
using IOT.Entities.DTO;
using IOT.Entities.Models;

namespace IOT.Business.Services
{
    public class GlobalAdminService : IGlobalAdminService
    {
        private readonly IGlobalAdminRepository _globalAdminRepository;

        public GlobalAdminService(IGlobalAdminRepository globalAdminRepository)
        {
            _globalAdminRepository = globalAdminRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetAllAdminsAsync()
        {
            var admins = await _globalAdminRepository.GetAllAsync();
            return admins.Select(a => a.ToDto());
        }

        public async Task<UserDTO> GetAdminByIdAsync(int id)
        {
            var admin = await _globalAdminRepository.GetByIdAsync(id);
            return admin.ToDto();
        }

        public async Task CreateAdminAsync(UserDTO adminDto)
        {
            var admin = adminDto.ToEntity();
            await _globalAdminRepository.AddAsync(admin);
        }

        public async Task UpdateAdminAsync(UserDTO adminDto)
        {
            var admin = adminDto.ToEntity();
            await _globalAdminRepository.UpdateAsync(admin);
        }

        public async Task DeleteAdminAsync(int id)
        {
            await _globalAdminRepository.DeleteAsync(id);
        }
    }
}
