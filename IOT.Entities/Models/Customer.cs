using IOT.Entities.Request;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class Customer
    {
        public Customer()
        {
            
        }

        public Customer(CustomerRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            CustomerName = request.CustomerName;
            CustomerPhone = request.CustomerPhone;
            CustomerEmail = request.CustomerEmail;
            CustomerCity = request.CustomerCity;
            CustomerRegion = request.CustomerRegion;

            Sites = request.Sites != null ? request.Sites.Select(site => new Site
            {
                SiteName = site.SiteName,
                SiteLocation = site.SiteLocation,
                Latitude = site.Latitude,
                Longitude = site.Longitude,
                Devices = site.Devices != null ? site.Devices.Select(device => new Device
                {
                    DeviceName = device.DeviceName,
                    ProductType = device.ProductType,
                    ThreSholdValue = device.ThreSholdValue
                }).ToList() : new List<Device>()
            }).ToList() : new List<Site>();
            CustomerUsers = request.CustomerUsers != null ? new List<string>(request.CustomerUsers) : new List<string>();
            DigitalServices = request.DigitalServices != null ? request.DigitalServices.Select(service => new DigitalService
            {
                ServiceStartDate = service.ServiceStartDate,
                ServiceEndDate = service.ServiceEndDate,
                IsActive = service.IsActive,
                NotificationUsers = service.NotificationUsers != null ? new List<string>(service.NotificationUsers) : new List<string>()
            }).ToList() : new List<DigitalService>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("CustomerID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid CustomerID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Phone]
        public string CustomerPhone { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerCity { get; set; }

        [Required]
        public string CustomerRegion { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<Site> Sites { get; set; } = new List<Site>();
        public List<string> CustomerUsers { get; set; } = new List<string>();
        public List<DigitalService> DigitalServices { get; set; } = new List<DigitalService>();
    }

    public class Site
    {
        [BsonElement("SiteID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid SiteID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string SiteName { get; set; }

        [Required]
        [StringLength(200)]
        public string SiteLocation { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public List<Device> Devices { get; set; } = new List<Device>();
    }

    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [Required]
        public Guid DeviceID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; }

        [Required]
        public string ProductType { get; set; }

        public double ThreSholdValue { get; set; }
    }

    public class DigitalService
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [Required]
        public Guid DigitalServiceID { get; set; } = Guid.NewGuid();

        [Required]
        public DateTime ServiceStartDate { get; set; }

        [Required]
        public DateTime ServiceEndDate { get; set; }

        public bool IsActive { get; set; }

        public List<string> NotificationUsers { get; set; } = new List<string>();
    }
}
