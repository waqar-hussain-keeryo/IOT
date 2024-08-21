using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalAdminController : ControllerBase
    {
        private readonly IGlobalAdminService _globalAdminService;
        public GlobalAdminController(IGlobalAdminService globalAdminService)
        {
            _globalAdminService = globalAdminService;
        }

        
        [HttpPost("RegisterAdmin")]
        [AllowAnonymous] // Allow anonymous access for first-time registration
        public async Task<IActionResult> RegisterAdmin([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return BadRequest("Admin data is required.");
            }

            try
            {
                await _globalAdminService.RegisterGlobalAdmin(user);
                return Ok("Global admin registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            try
            {
                var token = await _globalAdminService.RegisterUser(userDto);
                return Ok(new { Token = token });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var token = await _globalAdminService.Login(email, password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
