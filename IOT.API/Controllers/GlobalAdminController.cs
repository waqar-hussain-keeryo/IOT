using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
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

        [HttpPost("RegisterAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Admin data is required."
                });
            }

            try
            {
                var response = await _globalAdminService.RegisterGlobalAdmin(user);

                return response.Success
                    ? Ok(new ApiResponse
                    {
                        Success = true,
                        Message = response.Message,
                        Data = response.Data
                    })
                    : BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = response.Message
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred."
                });
            }
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _userService.GetAllUsers();

                return response.Success
                    ? Ok(new ApiResponse
                    {
                        Success = true,
                        Message = response.Message ?? "Users retrieved successfully.",
                        Data = response.Data
                    })
                    : BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = response.Message
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO role)
        {
            try
            {
                if (role == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Role object is null."
                    });
                }

                // Call the service method to create the role
                var response = await _userService.CreateRole(role);

                return response.Success
                    ? Ok(new ApiResponse
                    {
                        Success = true,
                        Message = response.Message,
                        Data = response.Data
                    })
                    : BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = response.Message
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred: " + ex.Message
                });
            }
        }

    }
}
