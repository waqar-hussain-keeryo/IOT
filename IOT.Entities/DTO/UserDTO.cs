using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using IOT.Entities.Models;

namespace IOT.Entities.DTO
{
    public class UserDTO
    {
        public UserDTO() { }

        public UserDTO(Users user)
        {
            UserID = user.UserID;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            RoleID = user.RoleID;
            CustomerID = user.CustomerID;
            Password = user.Password;
            EmailVerified = user.EmailVerified;
            IsDeleted = user.IsDeleted;
        }

        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid RoleID { get; set; }
        public string? RoleName { get; set; }
        public Guid? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string Password { get; set; }
        public bool EmailVerified { get; set; }
        public bool IsDeleted { get; set; }
        public object? Token { get; set; }
    }
}
