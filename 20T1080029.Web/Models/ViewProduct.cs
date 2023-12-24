using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class ViewProduct : PaginationSearchInput
    {
        public Product? product { get; set; }
        public List<ProductAttribute>? productAttributes { get; set; }
        public List<ProductPhoto>? productPhotos { get; set; }

        public PaginationSearchProductOutput? data1 { get; set; }
    }
}
