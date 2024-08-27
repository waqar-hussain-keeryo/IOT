using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class DigitalServiceRequest
    {
        [Required]
        public DateTime ServiceStartDate { get; set; }

        [Required]
        public DateTime ServiceEndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
