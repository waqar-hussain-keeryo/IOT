using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class Role
    {
        [Required]
        public string RoleID { get; set; }

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
    }
}
