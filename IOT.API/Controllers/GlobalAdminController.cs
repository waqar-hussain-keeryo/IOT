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
            try
            {
                if (user == null)
                    return BadRequest("Admin data is required.");

                await _globalAdminService.RegisterGlobalAdmin(user);
                return Ok("Global admin registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();
                
                if (users == null || !users.Any())
                    return NotFound("No users found.");

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO role)
        {
            try
            {
                if (role == null)
                    return BadRequest("Role object is null");

                await _userService.CreateRole(role);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
