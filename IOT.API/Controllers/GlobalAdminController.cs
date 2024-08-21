using IOT.Business.Interfaces;
using IOT.Business.Services;
using IOT.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GlobalAdminController : ControllerBase
    {
        private readonly IGlobalAdminService _globalAdminService;

        public GlobalAdminController(IGlobalAdminService globalAdminService)
        {
            _globalAdminService = globalAdminService;
        }

        [HttpPost("CreateCustomer")]
        [Authorize(Roles = "GlobalAdmin")]
        public IActionResult CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            var result = _globalAdminService.CreateCustomer(customerDto);
            if (result.IsValid)
                return Ok(result);
            else
                return BadRequest(result.Errors);
        }
    }
}
