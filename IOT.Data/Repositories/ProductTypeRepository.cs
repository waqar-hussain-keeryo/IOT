using IOT.Entities.Models;
using MongoDB.Driver;

namespace IOT.Data.Repositories
{
    public interface IProductTypeRepository
    {
        Task<ProductType> Create(ProductType productType);
        Task<ProductType> Update(Guid productTypeId, ProductType request);
        Task<bool> Delete(Guid productTypeId);
        Task<ProductType> GetById(Guid productTypeId);
        Task<IEnumerable<ProductType>> GetAll();
        Task<ProductType> GetByName(string productTypeName);
    }

    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly MongoDBContext _dbContext;

        public ProductTypeRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductType> Create(ProductType productType)
        {
            await _dbContext.ProductTypes.InsertOneAsync(productType);
            return productType;
        }

        public async Task<ProductType> Update(Guid productTypeId, ProductType request)
        {
            var updateDefinition = Builders<ProductType>.Update
                .Set(pt => pt.ProductTypeName, request.ProductTypeName)
                .Set(pt => pt.MinVal, request.MinVal)
                .Set(pt => pt.MaxVal, request.MaxVal)
                .Set(pt => pt.UOM, request.UOM)
                .Set(pt => pt.IsActive, request.IsActive);

            var filter = Builders<ProductType>.Filter.Eq(pt => pt.ProductTypeID, productTypeId);

            var updateResult = await _dbContext.ProductTypes.UpdateOneAsync(filter, updateDefinition);

            if (updateResult.ModifiedCount > 0)
            {
                return await _dbContext.ProductTypes.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<bool> Delete(Guid productTypeId)
        {
            var filter = Builders<ProductType>.Filter.Eq(pt => pt.ProductTypeID, productTypeId);
            var deleteResult = await _dbContext.ProductTypes.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        public async Task<ProductType> GetById(Guid productTypeId)
        {
            return await _dbContext.ProductTypes.Find(pt => pt.ProductTypeID == productTypeId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAll()
        {
            return await _dbContext.ProductTypes.Find(_ => true).ToListAsync();
        }

        public async Task<ProductType> GetByName(string productTypeName)
        {
            var filter = Builders<ProductType>.Filter.Eq(pt => pt.ProductTypeName, productTypeName);
            return await _dbContext.ProductTypes.Find(filter).FirstOrDefaultAsync();
        }
    }
}
