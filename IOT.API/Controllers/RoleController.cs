using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetRoleByName")]
        public async Task<IActionResult> GetRoleByName(string roleName)
        {
            var role = await _roleService.GetRoleByName(roleName);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO role)
        {
            if (role == null)
            {
                return BadRequest("Role object is null");
            }

            await _roleService.CreateRole(role);
            return CreatedAtAction(nameof(GetRoleByName), new { roleName = role.RoleName }, role);
        }
    }
}
