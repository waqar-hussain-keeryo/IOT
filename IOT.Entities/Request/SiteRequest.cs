using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class SiteRequest
    {
        [Required]
        [StringLength(100)]
        public string SiteName { get; set; }

        [Required]
        [StringLength(200)]
        public string SiteLocation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
