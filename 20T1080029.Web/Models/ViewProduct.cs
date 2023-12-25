using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class ViewProduct : PaginationSearchInput
    {
        public Product? Product { get; set; }
        public List<ProductAttribute>? ProductAttributes { get; set; }
        public List<ProductPhoto>? ProductPhotos { get; set; }

        public PaginationSearchProductOutput? Data1 { get; set; }
    }
}
