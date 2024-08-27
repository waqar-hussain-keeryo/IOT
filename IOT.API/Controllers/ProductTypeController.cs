using IOT.Business.Interfaces;
using IOT.Entities.DTO;
using IOT.Entities.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IOT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductTypeController(IProductTypeService productTypeService, IHttpContextAccessor httpContextAccessor)
        {
            _productTypeService = productTypeService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateProductType")]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Product type data is required."));
                }

                var response = await _productTypeService.CreateProductType(request);

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

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateProductType")]
        public async Task<IActionResult> UpdateProductType(Guid productTypeId, [FromBody] ProductTypeRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse(false, "Product type data is required."));
                }

                var response = await _productTypeService.UpdateProductType(productTypeId, request);

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

        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteProductType")]
        public async Task<IActionResult> DeleteProductType(Guid productTypeId)
        {
            try
            {
                var response = await _productTypeService.DeleteProductType(productTypeId);

                return response.Success
                    ? Ok(new ApiResponse(true, response.Message))
                    : BadRequest(new ApiResponse(false, response.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   new ApiResponse(false, "An unexpected error occurred: " + ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetProductTypeById")]
        public async Task<IActionResult> GetProductTypeById(Guid productTypeId)
        {
            try
            {
                var response = await _productTypeService.GetProductTypeById(productTypeId);

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

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllProductTypes")]
        public async Task<IActionResult> GetAllProductTypes([FromBody] PaginationRequest request)
        {
            try
            {
                var response = await _productTypeService.GetAllProductTypes(request);

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
