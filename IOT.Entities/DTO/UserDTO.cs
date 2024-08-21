using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.DTO
{
    public class UserDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("UserID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid UserID { get; set; } = Guid.NewGuid();

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
        [StringLength(100)]
        public string RoleName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? CustomerID { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 6)]
        public string Password { get; set; }

        public bool EmailVerified { get; set; }
    }
}
