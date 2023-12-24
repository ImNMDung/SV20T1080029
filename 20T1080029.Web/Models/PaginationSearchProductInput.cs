namespace SV20T1080029.Web.Models
{
    /// <summary>
    /// thông tin đầ uvào dùng để tìm kiếm phân trang 
    /// </summary>
    public class PaginationSearchProductInput : PaginationSearchInput

    {

        public int supplierID { get; set; } = 0;

        public int categoryID { get; set; } = 0;
        public int minPrice { get; set; } = 0;
        public long maxPrice { get; set; } = 9999999999;
    }
}
