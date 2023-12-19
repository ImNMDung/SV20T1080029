using SV20T1080029.DomainModels;


namespace SV20T1080029.Web.Models
{
    public class PaginationSearchProductOutput : PaginationSearchBaseResult
    {
        public IList<Product> Data { get; set; }
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }

    }
}
