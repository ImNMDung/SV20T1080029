using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web;
using SV20T1080029.Web.Models;
using System.Drawing.Printing;

namespace SV20T1080029.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class OrderController : Controller
    {
        private const string Order_Search = "Order_Search";
        public const int Page_Size = 10;
        /// Hiển thị danh sách đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchOrderInput>(Order_Search);
            if (input == null)
            {
                input = new PaginationSearchOrderInput()
                {
                    Page = 1,
                    Status = 0,
                    PageSize = Page_Size,
                    SearchValue = ""

                };
            }

            return View(input);
        }
        public IActionResult Search(PaginationSearchOrderInput input)
        {

            int rowCount = 0;
            var data = OrderService.ListOrders(input.Page, input.PageSize, input.Status, input.SearchValue, out rowCount);
            var model = new PaginationSearchOrder()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
            ApplicationContext.SetSessionData(Order_Search, input);//lưu lại điều kiện tìm kiếm

            string errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;
            string deletedMessage = Convert.ToString(TempData["DeletedMessage"]);
            ViewBag.DeletedMessage = deletedMessage;
            string savedMessage = Convert.ToString(TempData["SavedMessage"]);
            ViewBag.SavedMessage = savedMessage;

            return View(model);
        }
        /// <summary>
        /// Giao diện trang tạo đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Giao diện trang chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id = 0 )
        {
            //Code chức năng lấy và hiển thị thông tin của đơn hàng và chi tiết của đơn hàng
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            // lấy thông tin của một đơn hàng và chi tiết đơn hàng đó theo mã đơn hàng
            var order = OrderService.GetOrder(id);
            var orderDetails = OrderService.ListOrderDetails(id);
            

            if (order == null || orderDetails
                 == null)
            {
                return RedirectToAction("Index");
            }
            var result = new ViewOrder()
            {
                Order = order,
                OrderDetails = orderDetails
            };
          
            return View(result);
        }
        /// <summary>
        /// Giao diện cập nhật thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>        
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            try
            {
                if (id < 0)
                {
                    // Chuyển hướng về action Index nếu id không hợp lệ
                    return RedirectToAction("Index");
                }

                if (productId < 0)
                {
                    // Chuyển hướng về action Details nếu productId không hợp lệ
                    return RedirectToAction("Details", new { id });
                }

                // Lấy thông tin chi tiết đơn hàng từ OrderService
                var orderDetail = OrderService.GetOrderDetail(id, productId);

                if (orderDetail == null)
                {
                    // Nếu không tìm thấy chi tiết đơn hàng, chuyển hướng về action Index
                    return RedirectToAction("Index");
                }

                // Trả về view với dữ liệu chi tiết đơn hàng
                return View(orderDetail);
            }
            catch (Exception ex)
            {
                // Log lỗi và xử lý tùy theo yêu cầu của bạn
                
                return RedirectToAction("Error");
                throw; // Ném lại exception để xem thông báo lỗi chi tiết trong môi trường phát triển
            }
        }
        /// <summary>
        /// Cập nhật chi tiết đơn hàng (trong giỏ hàng)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>
        [HttpPost]        
        public IActionResult UpdateDetail(int id = 0, int productId = 0, int quantity = 0, decimal salePrice = 0)
        {
            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>        
        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            //DONE: Code chức năng xóa 1 chi tiết trong đơn hàng
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            if (productID < 0)
            {
                return RedirectToAction("Details", new { id = id });
            }

            // Xoá chi tiết 1 đơn hàng nếu kiểm tra đúng hết
            bool isDeleted = OrderService.DeleteOrderDetail(id, productID);
            if (!isDeleted)
            {
              //  TempData[ERROR_MESSAGE] = "Không thể xoá mặt hàng này";
                return RedirectToAction("Details", new { id = id });
            }
           

            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //DONE: Code chức năng để xóa đơn hàng (nếu được phép xóa)
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            Order data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            // Xoá đơn hàng ở trạng thái vừa tạo, bị huỷ hoặc bị từ chối
            if (data.Status == OrderStatus.INIT
                || data.Status == OrderStatus.CANCEL
                || data.Status == OrderStatus.REJECTED)
            {
                OrderService.DeleteOrder(id);
                return RedirectToAction("Index");
            }
            return RedirectToAction($"Details", new {data.OrderID});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 


        

        
        public IActionResult Accept(int id = 0)
        {
            //DONE: Code chức năng chấp nhận đơn hàng (nếu được phép)
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }
            var  data = OrderService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            } else { 
            bool isAccepted = OrderService.AcceptOrder(id);
            if (!isAccepted)
            {
             //   TempData[ERROR_MESSAGE] = $"Chấp nhận đơn hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
                return RedirectToAction($"Details", new { data.OrderID  });
            }
            }
            return RedirectToAction("Details", new { id = id });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {            
            if (Request.Method == "GET")
                return View();
            else
            {
                //TODO: Chuyển đơn hàng cho người giao hàng

                return RedirectToAction("Details", new { id = id });
            } 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            //TODO: Ghi nhận hoàn tất đơn hàng

            return RedirectToAction($"Details", new { id = id });
        }
        /// <summary>
        /// Hủy bỏ đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            //TODO: Hủy đơn hàng

            return RedirectToAction($"Details", new { id = id });
        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            //TODO: Từ chối đơn hàng

            return RedirectToAction($"Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult SearchProducts()
        {
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Bổ sung thêm hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart()
        {            
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>        
        /// <returns></returns>
        public ActionResult RemoveFromCart()
        {            
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Xóa toàn bộ dữ liệu trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCart()
        {            
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Khởi tạo đơn hàng và chuyển đến trang Details sau khi khởi tạo xong để tiếp tục quá trình xử lý đơn hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public ActionResult Init()
        {
            int orderId = 111;

            //TODO: Khởi tạo đơn hàng và nhận mã đơn hàng được khởi tạo

            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
