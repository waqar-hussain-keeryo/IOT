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


        public async Task<ResponseDTO> AddSites(string roleName, Guid customerId, SiteRequest request)
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

                existingCustomer.Sites.Add(new Entities.Models.Site()
                {
                    SiteName = request.SiteName,
                    SiteLocation = request.SiteLocation,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                });

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Site successfully created.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateSites(string roleName, Guid customerId, Guid siteId, SiteRequest request)
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

                var site = existingCustomer.Sites.FirstOrDefault(s => s.SiteID == siteId);
                if (site == null)
                {
                    return new ResponseDTO(false, "Site not found.");
                }

                site.SiteName = request.SiteName;
                site.SiteLocation = request.SiteLocation;
                site.Latitude = request.Latitude;
                site.Longitude = request.Longitude;

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Site successfully updated.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> AddDevices(string roleName, Guid customerId, Guid siteId, DeviceRequest request)
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
                    return new ResponseDTO(false, "Device data required.");
                }

                var site = existingCustomer.Sites.FirstOrDefault(s => s.SiteID == siteId);
                if (site == null)
                {
                    return new ResponseDTO(false, "Site not found.");
                }

                site.Devices.Add(new Entities.Models.Device()
                {
                    DeviceName = request.DeviceName,
                    ProductType = request.ProductType,
                    ThreSholdValue = request.ThreSholdValue
                });

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Device successfully inserted.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateDevices(string roleName, Guid customerId, Guid siteId, Guid deviceId, DeviceRequest request)
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
                    return new ResponseDTO(false, "Device data required.");
                }

                var site = existingCustomer.Sites.FirstOrDefault(s => s.SiteID == siteId);
                if (site == null)
                {
                    return new ResponseDTO(false, "Site not found.");
                }

                var device = site.Devices.FirstOrDefault(d => d.DeviceID == deviceId);
                if (device == null)
                {
                    return new ResponseDTO(false, "Device not found.");
                }

                device.DeviceName = request.DeviceName;
                device.ProductType = request.ProductType;
                device.ThreSholdValue = request.ThreSholdValue;

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);

                return new ResponseDTO(true, "Device successfully updated.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> AddCustomerUsers(string roleName, Guid customerId, List<string> customerUsers)
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

                foreach (var user in customerUsers)
                {
                    if (!existingCustomer.CustomerUsers.Contains(user))
                    {
                        existingCustomer.CustomerUsers.Add(user);
                    }
                }

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);

                return new ResponseDTO(true, "Customer users successfully added.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> AddDigitalServices(string roleName, Guid customerId, DigitalServiceRequest request)
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
                    return new ResponseDTO(false, "Digital Service data required.");
                }

                existingCustomer.DigitalServices.Add(new Entities.Models.DigitalService()
                {
                    ServiceStartDate = request.ServiceStartDate,
                    ServiceEndDate = request.ServiceEndDate,
                    IsActive = request.IsActive
                });

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Digital Service successfully created.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateDigitalServices(string roleName, Guid customerId, Guid digitalServiceId, DigitalServiceRequest request)
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
                    return new ResponseDTO(false, "Digital Service data required.");
                }

                var digitalService = existingCustomer.DigitalServices.FirstOrDefault(s => s.DigitalServiceID == digitalServiceId);
                if (digitalService == null)
                {
                    return new ResponseDTO(false, "Digital Service not found.");
                }

                digitalService.ServiceStartDate = request.ServiceStartDate;
                digitalService.ServiceEndDate = request.ServiceEndDate;
                digitalService.IsActive = request.IsActive;

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);
                return new ResponseDTO(true, "Digital Service successfully updated.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

        public async Task<ResponseDTO> AddNotificationUsers(string roleName, Guid customerId, Guid digitalServiceId, List<string> notificationUsers)
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

                var digitalService = existingCustomer.DigitalServices.FirstOrDefault(ds => ds.DigitalServiceID == digitalServiceId);
                if (digitalService == null)
                {
                    return new ResponseDTO(false, "Digital service not found.");
                }

                foreach (var user in notificationUsers)
                {
                    if (!digitalService.NotificationUsers.Contains(user))
                    {
                        digitalService.NotificationUsers.Add(user);
                    }
                }

                var customerResponse = await _customerRepository.Update(existingCustomer.CustomerID, existingCustomer);
                var customerDTO = new CustomerDTO(customerResponse);

                return new ResponseDTO(true, "Notification users successfully added.", customerDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, "An unexpected error occurred: " + ex.Message);
            }
        }

    }
}
