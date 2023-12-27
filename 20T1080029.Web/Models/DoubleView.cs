namespace SV20T1080029.Web.Models
{
    public class DoubleView : PaginationSearchBaseResult
    {

        public PaginationSearchProductOutput? data1 { get; set; }

        public DomainModels.Product? data2 { get; set; }
    }
}
