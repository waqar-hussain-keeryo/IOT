using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
