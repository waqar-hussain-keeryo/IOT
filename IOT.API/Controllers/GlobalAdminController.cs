using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IOT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalAdminController : ControllerBase
    {
        private readonly IGlobalAdminService _globalAdminService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalAdminController(IGlobalAdminService globalAdminService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _globalAdminService = globalAdminService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRequest user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new ApiResponse(false, "Admin data is required."));
                }

                var response = await _globalAdminService.CreateAdmin(user);

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
        [HttpPut("UpdateAdmin")]
        public async Task<IActionResult> UpdateAdmin(string email, [FromBody] UserRequest user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new ApiResponse(false, "Admin data is required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.UpdateAdmin(email, roleName, user);

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
        [HttpPost("DeleteAdmin")]
        public async Task<IActionResult> DeleteAdmin(string email)
        {
            try
            {
                if (email == null)
                {
                    return BadRequest(new ApiResponse(false, "Email required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.DeleteAdmin(email, roleName);

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
        [HttpPost("GetAllAdmin")]
        public async Task<IActionResult> GetAllAdmin(PaginationRequest request)
        {
            try
            {
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.GetAllAdmin(roleName, request);

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
        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(PaginationRequest request)
        {
            try
            {
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.GetAllUsers(roleName,request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message ?? "Users retrieved successfully.", response.Data))
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
                var response = await _globalAdminService.GetAllCustomers(roleName, request);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message ?? "Customers retrieved successfully.", response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("RegisterCustomerAccount")]
        public async Task<IActionResult> RegisterCustomerAccount([FromBody] UserRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer data is required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.CreateCustomer(roleName, request);

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
        [HttpPost("UpdateCustomerAccount")]
        public async Task<IActionResult> UpdateCustomerAccount(string email, [FromBody] UserRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Admin data is required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.UpdateCustomer(email, roleName, request);

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
        [HttpPost("DeleteCustomerAccount")]
        public async Task<IActionResult> DeleteCustomerAccount(string email)
        {
            try
            {
                if (email == null)
                {
                    return BadRequest(new ApiResponse(false, "Email required."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.DeleteCustomer(email, roleName);

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
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest(new ApiResponse(false, "Role object is null."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.CreateRole(roleName, role);

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
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(string updateRoleName, [FromBody] RoleRequest role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest(new ApiResponse(false, "Invalid role object."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.UpdateRole(roleName, updateRoleName, role);

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
        [HttpPost("DeleteRole")]
        public async Task<IActionResult> DeleteRole(string deleteRoleName)
        {
            try
            {
                if (deleteRoleName == null)
                {
                    return BadRequest(new ApiResponse(false, "Enter Role Name."));
                }

                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _globalAdminService.DeleteRole(roleName, deleteRoleName);

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
