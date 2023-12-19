using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class PaginationSearchSuppliers : PaginationSearchBaseResult
    {
        public IList<Supplier> Data { get; set; }
    }
}
