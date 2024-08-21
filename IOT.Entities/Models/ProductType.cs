using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class ProductType
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("ProductTypeID")]
        [BsonRepresentation(BsonType.String)]
        [Required]
        public Guid ProductTypeID { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string ProductTypeName { get; set; }

        public bool IsActive { get; set; }

        public double MinVal { get; set; }

        public double MaxVal { get; set; }

        [Required]
        [StringLength(20)]
        public string UOM { get; set; }
    }
}
