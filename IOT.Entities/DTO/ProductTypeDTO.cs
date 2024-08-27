using IOT.Entities.Models;

namespace IOT.Entities.DTO
{
    public class ProductTypeDTO
    {
        public ProductTypeDTO() { }

        public ProductTypeDTO(ProductType product)
        {
            ProductTypeID = product.ProductTypeID;
            ProductTypeName = product.ProductTypeName;
            MinVal = product.MinVal;
            MaxVal = product.MaxVal;
            UOM = product.UOM;
            IsActive = product.IsActive;
        }

        public Guid ProductTypeID { get; set; }
        public string ProductTypeName { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }
        public string UOM { get; set; }
        public bool IsActive { get; set; }
    }
}
