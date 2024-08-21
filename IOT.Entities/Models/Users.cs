using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class Users
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; } 

        public string? CustomerID { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 6)]
        public string Password { get; set; }

        public bool EmailVerified { get; set; }
    }
}
