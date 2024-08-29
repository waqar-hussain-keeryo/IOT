using IOT.Entities.Models;
using IOT.Entities.Request;
using MongoDB.Driver;

namespace IOT.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> Create(Customer customer);
        Task<Customer> Update(Guid customerId, Customer request);
        Task<bool> Delete(Guid customerId);
        Task<Customer> GetById(Guid customerId);
        Task<Customer> GetByEmail(string email);
        Task<(IEnumerable<Customer> Customers, int TotalRecords)> GetAll(PaginationRequest request);


        Task<Customer> AddSite(Guid customerId, Entities.Models.Site request);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly MongoDBContext _dbContext;
        public CustomerRepository(MongoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Customer> Create(Customer customer)
        {
            await _dbContext.Customers.InsertOneAsync(customer);
            return customer;
        }

        public async Task<Customer> Update(Guid customerId, Customer request)
        {
            var updatedList = Builders<Customer>.Update;
            var updates = new List<UpdateDefinition<Customer>>();

            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                updates.Add(updatedList.Set(c => c.CustomerName, request.CustomerName));
            }

            if (!string.IsNullOrEmpty(request.CustomerPhone))
            {
                updates.Add(updatedList.Set(c => c.CustomerPhone, request.CustomerPhone));
            }

            if (!string.IsNullOrEmpty(request.CustomerEmail))
            {
                updates.Add(updatedList.Set(c => c.CustomerEmail, request.CustomerEmail));
            }

            if (!string.IsNullOrEmpty(request.CustomerCity))
            {
                updates.Add(updatedList.Set(c => c.CustomerCity, request.CustomerCity));
            }

            if (!string.IsNullOrEmpty(request.CustomerRegion))
            {
                updates.Add(updatedList.Set(c => c.CustomerRegion, request.CustomerRegion));
            }

            if (request.IsActive != default)
            {
                updates.Add(updatedList.Set(c => c.IsActive, request.IsActive));
            }

            if (request.Sites != null && request.Sites.Any())
            {
                updates.Add(updatedList.Set(c => c.Sites, request.Sites));
            }

            if (request.CustomerUsers != null && request.CustomerUsers.Any())
            {
                updates.Add(updatedList.Set(c => c.CustomerUsers, request.CustomerUsers));
            }

            if (request.DigitalServices != null && request.DigitalServices.Any())
            {
                updates.Add(updatedList.Set(c => c.DigitalServices, request.DigitalServices));
            }


            var combinedUpdate = updatedList.Combine(updates);
            var filter = Builders<Customer>.Filter.Eq(c => c.CustomerID, customerId);
            var updateResult = await _dbContext.Customers.UpdateOneAsync(filter, combinedUpdate);

            // Check if the update was successful
            if (updateResult.ModifiedCount > 0)
            {
                return await _dbContext.Customers.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }

        public async Task<bool> Delete(Guid customerId)
        {
            var updateRole = Builders<Customer>.Update
                .Set(r => r.IsDeleted, true)
                .Set(r => r.IsActive, true);
            var updateResult = await _dbContext.Customers.UpdateOneAsync(c => c.CustomerID == customerId, updateRole);

            return updateResult.ModifiedCount > 0;
        }

        public async Task<Customer> GetById(Guid customerId)
        {
            return await _dbContext.Customers.Find(c => c.CustomerID == customerId).FirstOrDefaultAsync();
        }

        public async Task<Customer> GetByEmail(string email)
        {
            return await _dbContext.Customers.Find(c => c.CustomerEmail == email && c.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Customer> Customers, int TotalRecords)> GetAll(PaginationRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;

            var collection = _dbContext.Customers;
            var totalRecords = await collection.CountDocumentsAsync(_ => true);

            var customers = await collection.Find(_ => true)
                                  .Skip((request.PageNumber - 1) * request.PageSize)
                                  .Limit(request.PageSize)
                                  .ToListAsync();

            return (customers, (int)totalRecords);

        }

        public async Task<Customer> AddSite(Guid customerId, Entities.Models.Site request)
        {
            var filter = Builders<Customer>.Filter.Eq(c => c.CustomerID, customerId);
            var update = Builders<Customer>.Update.Push(c => c.Sites, new Entities.Models.Site
            {
                SiteName = request.SiteName,
                SiteLocation = request.SiteLocation,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            });

            var updateResult = await _dbContext.Customers.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount > 0)
            {
                return await _dbContext.Customers.Find(filter).FirstOrDefaultAsync();
            }

            return null;
        }
    }
}
