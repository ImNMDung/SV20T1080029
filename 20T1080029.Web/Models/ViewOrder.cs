using SV20T1080029.DomainModels;

namespace SV20T1080029.Web.Models
{
    public class ViewOrder
    {
        /// <summary>
        /// Lấy ra thông tin của đơn đặt hàng
        /// </summary>
        public Order? Order { get; set; }
        /// <summary>
        /// Lấy ra thông tin chi tiết của đơn đặt hàng
        /// </summary>
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
