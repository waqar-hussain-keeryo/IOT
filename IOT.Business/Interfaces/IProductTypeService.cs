using IOT.Entities.DTO;
using IOT.Entities.Request;

namespace IOT.Business.Interfaces
{
    public interface IProductTypeService
    {
        Task<ResponseDTO> CreateProductType(ProductTypeRequest request);
        Task<ResponseDTO> UpdateProductType(Guid productTypeId, ProductTypeRequest request);
        Task<ResponseDTO> DeleteProductType(Guid productTypeId);
        Task<ResponseDTO> GetProductTypeById(Guid productTypeId);
        Task<ResponseDTO> GetAllProductTypes(PaginationRequest request);
    }
}
