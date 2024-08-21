using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            try
            {
                var token = await _userService.RegisterUser(userDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
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

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO role)
        {
            if (role == null)
            {
                return BadRequest("Role object is null");
            }

            await _userService.CreateRole(role);
            return Ok();
        }
    }
}
