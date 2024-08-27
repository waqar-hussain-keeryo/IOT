using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class ProductTypeRequest
    {
        [Required]
        [StringLength(100)]
        public string ProductTypeName { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        [Required]
        [StringLength(20)]
        public string UOM { get; set; }
        public bool IsActive { get; set; }
    }
}
