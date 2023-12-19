using SV20T1080029.DomainModels;


namespace SV20T1080029.Web.Models
{
    public class PaginationSearchCategory : PaginationSearchBaseResult
    {
        public IList<Category> Data { get; set; }
    }
}
