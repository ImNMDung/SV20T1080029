namespace SV20T1080029.Web.Models
{
    /// <summary>
    /// thông tin đầ uvào dùng để tìm kiếm phân trang 
    /// </summary>
    public class PaginationSearchInput

    {
        public int Page { get; set; }
        public int PageSize { get; set; } = 20;

        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                {
                    c += 1;
                }

                return c;

            }
        }
    }
}
