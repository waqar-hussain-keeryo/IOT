using IOT.Business.Interfaces;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IOT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(ICustomerService customerService, IHttpContextAccessor httpContextAccessor)
        {
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer data is required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.CreateCustomer(roleName, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer data is required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.UpdateCustomer(roleName, customerId, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(Guid customerId)
        {
            try
            {
                if (customerId == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer Id required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.DeleteCustomer(roleName, customerId);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            try
            {
                if (customerId == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer Id required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.GetCustomerById(roleName, customerId);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers(PaginationRequest request)
        {
            try
            {
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.GetAllCustomers(roleName, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }



        [Authorize(Roles = "Admin")]
        [HttpPost("AddSite")]
        public async Task<IActionResult> AddSite(Guid customerId, SiteRequest request)
        {
            try
            {
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.CreateSites(roleName, customerId, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("UpdateSite")]
        public async Task<IActionResult> UpdateSite(Guid customerId, SiteRequest request)
        {
            try
            {
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _customerService.UpdateSites(roleName, customerId, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }
    }
}
