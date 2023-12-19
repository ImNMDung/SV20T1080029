using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class PaginationSearchShipper : PaginationSearchBaseResult
    {
        public IList<Shipper> Data { get; set; }
    }
}
