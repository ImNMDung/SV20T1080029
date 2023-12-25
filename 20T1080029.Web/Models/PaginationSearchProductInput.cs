namespace SV20T1080029.Web.Models
{
    /// <summary>
    /// thông tin đầ uvào dùng để tìm kiếm phân trang 
    /// </summary>
    public class PaginationSearchProductInput : PaginationSearchInput

    {

        public int SupplierID { get; set; } = 0;

        public int CategoryID { get; set; } = 0;
        public int MinPrice { get; set; } = 0;
        public long MaxPrice { get; set; } = 0;
    }
}
