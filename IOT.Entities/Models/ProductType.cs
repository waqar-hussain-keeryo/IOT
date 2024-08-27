using IOT.Entities.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IOT.Entities.Models
{
    public class ProductType
    {
        public ProductType() { }

        public ProductType(ProductTypeDTO product)
        {
            ProductTypeID = product.ProductTypeID;
            ProductTypeName = product.ProductTypeName;
            MinVal = product.MinVal;
            MaxVal = product.MaxVal;
            UOM = product.UOM;
            IsActive = product.IsActive;
        }

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
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        [Required]
        [StringLength(20)]
        public string UOM { get; set; }
        public bool IsActive { get; set; }
    }
}
