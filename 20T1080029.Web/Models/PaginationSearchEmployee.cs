
using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class PaginationSearchEmployee : PaginationSearchBaseResult
    {
        public IList<Employee> Data { get; set; }
    }
}
