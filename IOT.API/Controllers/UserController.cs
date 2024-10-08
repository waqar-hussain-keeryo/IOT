﻿using IOT.Business.Interfaces;
using IOT.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using IOT.Entities.Request;

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
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _userService.Login(request);

                return response.Success
                   ? Ok(new ApiResponse(true, response.Message, response.Data))
                   : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRequest request)
        {
            try
            {
                var currentRole = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var response = await _userService.CreateUser(userId, currentRole, request);

                return response.Success
                  ? Ok(new ApiResponse(true, response.Message, response.Data))
                  : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse(false, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred." + ex.Message));
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("GetAllUsersByCustomer")]
        public async Task<IActionResult> GetAllUsersByCustomer(PaginationRequest request)
        {
            try
            {
                var customerId = _httpContextAccessor.HttpContext.User.FindFirstValue("userId");
                var roleName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var response = await _userService.GetAllUsersByCustomerId(customerId, roleName, request);

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

        [Authorize]
        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ApiResponse(false, "Email cannot be null or empty."));
                }

                var response = await _userService.GetUserByEmail(email);

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

        [Authorize]
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserRequest userRequest)
        {
            try
            {
                if (userRequest == null)
                {
                    return BadRequest(new ApiResponse(false, "User input cannot be null."));
                }

                var currentRoleName = User.FindFirst(ClaimTypes.Role)?.Value;
                var response = await _userService.UpdateUser(userId, currentRoleName, userRequest);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message, response.Data))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred: " + ex.Message));
            }
        }

    }
}
