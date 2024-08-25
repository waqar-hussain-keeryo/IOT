using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalAdminController : ControllerBase
    {
        private readonly IGlobalAdminService _globalAdminService;
        private readonly IUserService _userService;

        public GlobalAdminController(IGlobalAdminService globalAdminService, IUserService userService)
        {
            _globalAdminService = globalAdminService;
            _userService = userService;
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

                var response = await _globalAdminService.RegisterAdmin(user);

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
        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] UserRequest user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new ApiResponse(false, "Customer data is required."));
                }

                var response = await _globalAdminService.RegisterCustomer(user);

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
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(PaginationRequest request)
        {
            try
            {
                var response = await _userService.GetAllUsers(request);

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
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleRequest role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest(new ApiResponse(false, "Role object is null."));
                }

                var response = await _userService.CreateRole(role);

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
        public async Task<IActionResult> UpdateRole(string roleName, [FromBody] RoleRequest role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest(new ApiResponse(false, "Invalid role object."));
                }

                var response = await _userService.UpdateRole(roleName, role);

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
