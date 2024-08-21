using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.DTO
{
    public class MasterVariables
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Tag { get; set; }

        [Required]
        [StringLength(50)]
        public string UOM { get; set; }

        [Required]
        [StringLength(50)]
        public string ChannelID { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductType { get; set; }

        public bool IsActive { get; set; }
    }
}
