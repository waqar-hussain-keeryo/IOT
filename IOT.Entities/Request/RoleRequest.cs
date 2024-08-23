using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Request
{
    public class RoleRequest
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}
