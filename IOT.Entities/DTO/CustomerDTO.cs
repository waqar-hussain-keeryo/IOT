using MongoDB.Bson;

namespace IOT.Entities.DTO
{
    public class CustomerDTO
    {
        public Guid CustomerID { get; set; }
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
        public ObjectId DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string ProductType { get; set; }
        public double ThreSholdValue { get; set; }
    }

    public class DigitalService
    {
        public ObjectId DigitalServiceID { get; set; }
        public DateTime ServiceStartDate { get; set; }
        public DateTime ServiceEndDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> NotificationUsers { get; set; } = new List<string>();
    }
}
