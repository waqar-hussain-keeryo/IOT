using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class Users
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
        [BsonRepresentation(BsonType.String)]
        public Guid RoleID { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid? CustomerID { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string Password { get; set; }
        public bool EmailVerified { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
    }
}
