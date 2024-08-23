using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using IOT.Entities.Models;

namespace IOT.Entities.DTO
{
    public class RoleDTO
    {
        public RoleDTO() { }

        public RoleDTO(Role role)
        {
            RoleID = role.RoleID;
            RoleName = role.RoleName;
            RoleDescription = role.RoleDescription;
        }
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}
