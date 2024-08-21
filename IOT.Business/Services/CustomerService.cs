using IOT.Business.Interfaces;
using IOT.Entities.DTO;

namespace IOT.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer.ToDto();
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => c.ToDto());
        }

        public async Task CreateCustomerAsync(CustomerDTO CustomerDTO)
        {
            var customer = CustomerDTO.ToEntity();
            await _customerRepository.AddAsync(customer);
        }

        public async Task UpdateCustomerAsync(CustomerDTO CustomerDTO)
        {
            var customer = CustomerDTO.ToEntity();
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteAsync(id);
        }
    }
}
