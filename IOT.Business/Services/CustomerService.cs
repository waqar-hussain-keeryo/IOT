using Amazon.Runtime.Internal;
using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

namespace IOT.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ResponseDTO> CreateCustomer(string roleName, CustomerRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetByEmail(request.CustomerEmail);
                if (existingCustomer != null)
                {
                    return new ResponseDTO(false, "Customer already registered.");
                }

                var newCustomer = new Customer(request);
                var customerResponse = await _customerRepository.Create(newCustomer);

                var role = await _userRepository.GetRoleByName("Customer");
                var customerRoleId = role?.RoleID ?? Guid.NewGuid();
                if (role == null)
                {
                    var newRole = new Role
                    {
                        RoleName = "Customer",
                        RoleDescription = "Customer role created."
                    };

                    var roleResponse = await _userRepository.CreateRole(newRole);
                    customerRoleId = roleResponse.RoleID;
                }

                var newUser = new Users
                {
                    FirstName = customerResponse.CustomerName,
                    LastName = "",
                    Email = customerResponse.CustomerEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword("customer123+"),
                    RoleID = customerRoleId,
                    CustomerID = customerResponse.CustomerID
                };

                var accountResponse = await _userRepository.CreateUser(newUser);
                var responseDTO = new UserDTO(accountResponse);

                return new ResponseDTO(true, "Customer successfully registered.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateCustomer(string roleName, Guid customerId, CustomerRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                if (request == null)
                {
                    return new ResponseDTO(false, "Customer data required.");
                }

                var newCustomer = new Customer(request);
                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, newCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Customer successfully updated.", customerDTO);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteCustomer(string roleName, Guid customerId)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                var customerResponse = await _customerRepository.Delete(existingCustomer.CustomerID);

                if (customerResponse == true)
                {
                    var currentUser = await _userRepository.GetUserByCustomerId(existingCustomer.CustomerID);
                    if (currentUser == null)
                    {
                        return new ResponseDTO(false, "User not found.");
                    }

                    currentUser.IsDeleted = true;
                    var updateResponse = await _userRepository.UpdateUser(currentUser.UserID, currentUser);
                }

                return new ResponseDTO(true, "Customer successfully deleted.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> GetCustomerById(string roleName, Guid customerId)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                var responseDTO = new CustomerDTO(existingCustomer);

                return new ResponseDTO(true, "Customer retrive successfully.", responseDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> GetAllCustomers(string roleName, PaginationRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var (customer, totalRecords) = await _customerRepository.GetAll(request);
                if (customer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                return new ResponseDTO(true, "Admin retrieved successfully.",
                    new
                    {
                        Customers = customer,
                        TotalRecords = totalRecords,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }


        public async Task<ResponseDTO> CreateSites(string roleName, Guid customerId, SiteRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                if (request == null)
                {
                    return new ResponseDTO(false, "Site data required.");
                }

                var customerResponse = await _customerRepository.CreateSite(existingCustomer.CustomerID, request);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Site successfully created.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateSites(string roleName, Guid customerId, SiteRequest request)
        {
            try
            {
                if (roleName != "Admin")
                {
                    return new ResponseDTO(false, "You are not authorized. Please contact your administrator.");
                }

                var existingCustomer = await _customerRepository.GetById(customerId);
                if (existingCustomer == null)
                {
                    return new ResponseDTO(false, "Customer not found.");
                }

                if (request == null)
                {
                    return new ResponseDTO(false, "Site data required.");
                }

                var siteId = existingCustomer.Sites.FirstOrDefault().SiteID;
                var customerResponse = await _customerRepository.UpdateSite(existingCustomer.CustomerID, siteId, request);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Site successfully updated.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public Task<ResponseDTO> AddDevices(string roleName, Guid customerId, DeviceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> AddCustomerUsers(string roleName, Guid customerId, List<string> CustomerUsers)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> CreateDigitalServices(string roleName, Guid customerId, DigitalServiceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO> AddNotificationUsers(string roleName, Guid customerId, List<string> NotificationUsers)
        {
            throw new NotImplementedException();
        }
    }
}
