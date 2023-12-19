using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class ViewProduct
    {
        public Product? product { get; set; }
        public ProductAttribute? productAttribute { get; set; }
        public ProductPhoto? productPhoto { get; set; }

        public IList<Product> Products { get; set; }
        public IList<ProductAttribute> ProductAttributes { get; set; }
        public IList<ProductPhoto> ProductPhotos { get; set; }
    }
}
