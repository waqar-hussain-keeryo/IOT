using IOT.Business.Interfaces;
using IOT.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IOT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var response = await _userService.Login(email, password);

                if (response.Success)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = "Login successful.",
                        Data = response.Data
                    });
                }
                else
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = response.Message
                    });
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
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


        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var response = await _userService.RegisterUser(user, userId);

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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("GetAllUsersByCustomer")]
        public async Task<IActionResult> GetAllUsersByCustomer(int pageNumber, int pageSize)
        {
            try
            {
                var customerId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");

                if (string.IsNullOrEmpty(customerId))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Customer ID not found."
                    });
                }

                // Retrieve all users by customer ID
                var response = await _userService.GetAllUsersByCustomerId(customerId, pageNumber, pageSize);

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

        [Authorize]
        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "Email cannot be null or empty."
                    });
                }

                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var response = await _userService.GetUserByEmail(email, userId);

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
