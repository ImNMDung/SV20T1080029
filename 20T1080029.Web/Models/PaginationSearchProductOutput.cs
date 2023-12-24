using SV20T1080029.DomainModels;


namespace SV20T1080029.Web.Models
{
    public class PaginationSearchProductOutput : PaginationSearchBaseResult
    {
        public IList<Product> Data { get; set; }
       
        public int supplierID { get; set; } = 0;

        public int categoryID { get; set; } = 0;
        public int minPrice { get; set; } = 0;
        public long maxPrice { get; set; } = 9999999999;

    }
}
