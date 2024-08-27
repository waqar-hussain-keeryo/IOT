using IOT.Entities.Models;
using IOT.Entities.Request;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.DTO
{
    public class CustomerDTO
    {
        public CustomerDTO()
        {

        }

        public CustomerDTO(Customer request)
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

        public Guid CustomerID { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerRegion { get; set; }
        public bool IsActive { get; set; }
        public List<Site> Sites { get; set; } = new List<Site>();
        public List<string> CustomerUsers { get; set; } = new List<string>();
        public List<DigitalService> DigitalServices { get; set; } = new List<DigitalService>();
    }

    public class Site
    {
        public string SiteName { get; set; }
        public string SiteLocation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
    }

    public class Device
    {
        public Guid DeviceID { get; set; } = Guid.NewGuid();
        public string DeviceName { get; set; }
        public string ProductType { get; set; }
        public double ThreSholdValue { get; set; }
    }

    public class DigitalService
    {
        public Guid DigitalServiceID { get; set; } = Guid.NewGuid();
        public DateTime ServiceStartDate { get; set; }
        public DateTime ServiceEndDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> NotificationUsers { get; set; } = new List<string>();
    }
}