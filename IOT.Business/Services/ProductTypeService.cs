using IOT.Business.Interfaces;
using IOT.Data.Repositories;
using IOT.Entities.DTO;
using IOT.Entities.Models;
using IOT.Entities.Request;

namespace IOT.Business.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly IProductTypeRepository _productTypeRepository;

        public ProductTypeService(IProductTypeRepository productTypeRepository)
        {
            _productTypeRepository = productTypeRepository;
        }

        public async Task<ResponseDTO> CreateProductType(ProductTypeRequest request)
        {
            try
            {
                var existingProductType = await _productTypeRepository.GetByName(request.ProductTypeName);
                if (existingProductType != null)
                {
                    return new ResponseDTO(false, "Product type already exists.");
                }

                var newProductType = new ProductType
                {
                    ProductTypeName = request.ProductTypeName,
                    MinVal = request.MinVal,
                    MaxVal = request.MaxVal,
                    UOM = request.UOM,
                    IsActive = request.IsActive
                };

                var createdProductType = await _productTypeRepository.Create(newProductType);
                var productTypeDTO = new ProductTypeDTO(createdProductType);

                return new ResponseDTO(true, "Product type created successfully.", productTypeDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> UpdateProductType(Guid productTypeId, ProductTypeRequest request)
        {
            try
            {
                var existingProductType = await _productTypeRepository.GetById(productTypeId);
                if (existingProductType == null)
                {
                    return new ResponseDTO(false, "Product type not found.");
                }

                existingProductType.ProductTypeName = request.ProductTypeName;
                existingProductType.MinVal = request.MinVal;
                existingProductType.MaxVal = request.MaxVal;
                existingProductType.UOM = request.UOM;
                existingProductType.IsActive = request.IsActive;

                var updatedProductType = await _productTypeRepository.Update(productTypeId, existingProductType);
                var productTypeDTO = new ProductTypeDTO(updatedProductType);

                return new ResponseDTO(true, "Product type updated successfully.", productTypeDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeleteProductType(Guid productTypeId)
        {
            try
            {
                var existingProductType = await _productTypeRepository.GetById(productTypeId);
                if (existingProductType == null)
                {
                    return new ResponseDTO(false, "Product type not found.");
                }

                var isDeleted = await _productTypeRepository.Delete(productTypeId);
                if (isDeleted)
                {
                    return new ResponseDTO(true, "Product type deleted successfully.");
                }

                return new ResponseDTO(false, "Failed to delete product type.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetProductTypeById(Guid productTypeId)
        {
            try
            {
                var productType = await _productTypeRepository.GetById(productTypeId);
                if (productType == null)
                {
                    return new ResponseDTO(false, "Product type not found.");
                }

                var productTypeDTO = new ProductTypeDTO(productType);
                return new ResponseDTO(true, "Product type retrieved successfully.", productTypeDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetAllProductTypes(PaginationRequest request)
        {
            try
            {
                var productTypes = await _productTypeRepository.GetAll();

                var productTypeDTOs = productTypes.Select(pt => new ProductTypeDTO(pt)).ToList();

                return new ResponseDTO(true, "Product types retrieved successfully.",
                    new
                    {
                        ProductTypes = productTypeDTOs,
                        TotalRecords = productTypeDTOs.Count,
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(false, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
