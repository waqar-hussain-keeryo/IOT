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
                var token = await _userService.Login(email, password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var token = await _userService.RegisterUser(userDto, userId);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("GetAllUsersByCustomer")]
        public async Task<IActionResult> GetAllUsersByCustomer()
        {
            try
            {
                var customerId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var users = await _userService.GetAllUsersByCustomerId(customerId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                if (user == null)
                    return NotFound("User not found.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
