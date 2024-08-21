using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class ProductType
    {
        [Required]
        public string ProductTypeID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductTypeName { get; set; }

        public bool IsActive { get; set; }

        public double MinVal { get; set; }

        public double MaxVal { get; set; }

        [Required]
        [StringLength(20)]
        public string UOM { get; set; }
    }
}
