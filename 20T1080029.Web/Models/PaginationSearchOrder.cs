using SV20T1080029.DomainModels;


namespace SV20T1080029.Web.Models
{
    public class PaginationSearchOrder : PaginationSearchBaseResult
    {
        public IList<Order>? Data { get; set; }
    }
}
