using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.DTO
{
    public class RoleDTO
    {
        [BsonElement("RoleID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid RoleID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }
    }
}
