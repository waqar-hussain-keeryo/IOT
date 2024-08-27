using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class DeviceRequest
    {
        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; }

        [Required]
        public string ProductType { get; set; }
        public double ThreSholdValue { get; set; }
    }
}
