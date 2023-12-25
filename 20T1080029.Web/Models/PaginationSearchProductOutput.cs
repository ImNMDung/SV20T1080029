using SV20T1080029.DomainModels;


namespace SV20T1080029.Web.Models
{
    public class PaginationSearchProductOutput : PaginationSearchBaseResult
    {
        public IList<Product> Data { get; set; }
       
        public int SupplierId { get; set; } = 0;

        public int CategoryId { get; set; } = 0;
        public int MinPrice { get; set; } = 0;
        public long MaxPrice { get; set; } = 9999999999;

    }
}
