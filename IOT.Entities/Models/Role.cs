using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("RoleID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid RoleID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
