using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web;
using SV20T1080029.Web.Models;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SV20T1080029.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "Shopping_Cart";
        private const string ERROR_MESSAGE = "Error_Message";
        private const string SESSION_CONDITION = "Order_Condition";

        private const string Order_Search = "Order_Search";
        public const int Page_Size = 10;
        private const string Product_Search = "Product_Search_Order";

        /// Hiển thị danh sách đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            int rowCount = 0;
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(Order_Search);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                   // Status = 0,
                    PageSize = Page_Size,
                    SearchValue = "",
                };
            }

            return View(input);
        }
        public IActionResult Search(PaginationSearchOrderInput input)
        {

            int rowCount = 0;
            var data = OrderDataService.ListOrders(input.Page, input.PageSize, input.Status, input.SearchValue ?? "", out rowCount);
            var model = new PaginationSearchOrder()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data

            };
           ApplicationContext.SetSessionData(Order_Search, input);//lưu lại điều kiện tìm kiếm

            //string errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            //ViewBag.ErrorMessage = errorMessage;
            //string deletedMessage = Convert.ToString(TempData["DeletedMessage"]);
            //ViewBag.DeletedMessage = deletedMessage;
            //string savedMessage = Convert.ToString(TempData["SavedMessage"]);
            //ViewBag.SavedMessage = savedMessage;

            return View(model);
        }
        /// <summary>
        /// Giao diện trang tạo đơn hàng
        /// </summary>
        /// <returns></returns>
       
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
            var order = OrderDataService.GetOrder(id);
            var orderDetails = OrderDataService.ListOrderDetails(id);
            

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
        /// Tạo đơn hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult CreateDetail(int id = 0)

        {
            var model = new OrderDetail()
            {
                OrderID = id,
                ProductID = 0
            };
            return View(model);
        }


        /// <summary>
        /// Giao diện cập nhật thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>    
        /// 




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
                var orderDetail = OrderDataService.GetOrderDetail(id, productId);

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
            // mã đặt hàng
            if (productId <= 0)
            {
                TempData[ERROR_MESSAGE] = "mã đặt hàng không tồn tại";
                return RedirectToAction("Details", new { id = id });
            }
            // Số lượng
            if (quantity < 1)
            {
                TempData[ERROR_MESSAGE] = "Số lượng không tồn tại";
                return RedirectToAction("Details", new { id = id });
            }

            // Đơn giá
            if (salePrice < 1)
            {
                TempData[ERROR_MESSAGE] = "Đơn giá không tồn tại";
                return RedirectToAction("Details", new { id = id });
            }

            // Cập nhật chi tiết 1 đơn hàng nếu kiểm tra đúng hết
            OrderDataService.SaveOrderDetail(id, productId, quantity, salePrice);
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
            bool isDeleted = OrderDataService.DeleteOrderDetail(id, productID);
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
            Order data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            // Xoá đơn hàng ở trạng thái vừa tạo, bị huỷ hoặc bị từ chối
            if (data.Status == OrderStatus.INIT
                || data.Status == OrderStatus.CANCEL
                || data.Status == OrderStatus.REJECTED)
            {
                OrderDataService.DeleteOrder(id);
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
            var  data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            } else { 
            bool isAccepted = OrderDataService.AcceptOrder(id);
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
        /// <param name="shipperID"></param>
        /// <param name="countProducts"></param>
        /// <returns></returns>
        public IActionResult Shipping(int id = 0, int shipperID = 0, int countProducts = 0)
        {
            //if (Request.Method == "GET")
            //    return View();
            //else
            //{
            //TODO: Chuyển đơn hàng cho người giao hàng

            //    return RedirectToAction("Details", new { id = id });
            //}

            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            if (Request.Method == "GET")
            {
                ViewBag.OrderID = id;
                ViewBag.CountProducts = countProducts;
                return View();
            }

            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            if (shipperID <= 0)
            {
                TempData[ERROR_MESSAGE] = "Bạn phải chọn đơn vị vận chuyển";
                return RedirectToAction($"Details", new { data.OrderID });
            }
            if (countProducts <= 0)
            {
                TempData[ERROR_MESSAGE] = "Không có mặt hàng nào để chuyển giao";
                return RedirectToAction($"Details", new { data.OrderID });
            }
            bool isShipped = OrderDataService.ShipOrder(id, shipperID);
            if (!isShipped)
            {
             //   TempData[ERROR_MESSAGE] = $"Xác nhận chuyển đơn hàng cho người giao hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
                return RedirectToAction($"Details", new {data.OrderID});
            }
            return RedirectToAction($"Details" , new {id = id});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            //TODO: Ghi nhận hoàn tất đơn hàng
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            bool isFinished = OrderDataService.FinishOrder(id);
            if (!isFinished)
            {
             //   TempData[ERROR_MESSAGE] = $"Xác nhận hoàn tất đơn hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
               

                return RedirectToAction($"Details", new { data.OrderID });
            }
           
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
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            Order data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            bool isCanceled = OrderDataService.CancelOrder(id);
            if (!isCanceled)
            {
              //  TempData[ERROR_MESSAGE] = $"Hủy bỏ đơn hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
             //   return RedirectToAction($"Details/{data.OrderID}");

                return RedirectToAction($"Details", new { data.OrderID });
            }
           
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

            //Code chức năng từ chối đơn hàng (nếu được phép)
            if (id < 0)
            {
                return RedirectToAction("Index");
            }

            var data = OrderDataService.GetOrder(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }

            bool isRejected = OrderDataService.RejectOrder(id);
            if (!isRejected)
            {
                //TempData[ERROR_MESSAGE] = $"Từ chối đơn hàng thất bại vì trạng thái đơn hàng hiện tại là: {data.StatusDescription}";
                return RedirectToAction($"Details",  new { data.OrderID}
            );
            }
            return RedirectToAction($"Details" , new { id = id});
        }

        /// <summary>
        /// Tạo đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
          
            ViewBag.ErrorMessage = TempData[ERROR_MESSAGE] ?? "";
            return View(GetShoppingCart());
        }
        /// <summary>
        /// Tìm kiếm hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult SearchProducts(int page = 1, string searchValue = "")
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchProductInput>(Product_Search);
            if (input == null)
            {
                input = new PaginationSearchProductInput()
                {
                    Page = 1,

                    PageSize = 6,
                    SearchValue = searchValue ?? "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                };
            }
            int rowCount = 0;
            var data = ProductDataService.ListProducts(page, 6, searchValue ?? "", 0, 0, 0, 0, out rowCount);
            ViewBag.Page = page;
            return View(data);

        }


        /// <summary>
        /// Lấy dữ liệu giả hàng
        /// </summary>
        /// <returns></returns>
        private List<OrderDetail> GetShoppingCart()
        {
            //  Thử lấy giỏ hàng từ dữ session.
            List<OrderDetail> shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);

            // Nếu giỏ hàng không được tìm thấy trong dữ liệu phiên làm việc, tạo mới.
            if (shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart); // Lưu ss
            }
            return shoppingCart;
        }



        /// <summary>
        /// Xóa toàn bộ dữ liệu trong giỏ hàng
        /// </summary>
        /// <returns></returns>

        public ActionResult ClearCart()
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }


        /// <summary>
        /// Bổ sung thêm hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart(OrderDetail data)
        {
            if (data == null)
            {
                TempData[ERROR_MESSAGE] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Create");
            }
            if (data.SalePrice <= 0 || data.Quantity <= 0)
            {
                TempData[ERROR_MESSAGE] = "Giá bán và số lượng không hợp lệ";
                return RedirectToAction("Create");
            }

            List<OrderDetail> shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);

            if (existsProduct == null) //Nếu mặt hàng cần được bổ sung chưa có trong giỏ hàng thì bổ sung vào giỏ
            {
                shoppingCart.Add(data);
            }
            else //Trường hợp mặt hàng cần bổ sung đã có thì tăng số lượng và thay đổi đơn giá
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
        /// <summary>
        /// Xóa 1 mặt hàng khỏi giỏ hàng
        /// </summary>        
        /// <returns></returns>
        public ActionResult RemoveFromCart(int id = 0)
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return RedirectToAction("Create");
        }
    
      

        /// <summary>
        /// Khởi tạo đơn hàng và chuyển đến trang Details sau khi khởi tạo xong để tiếp tục quá trình xử lý đơn hàng
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        public IActionResult Init(int customerId = 0, int employeeId =0)
        {
            List<OrderDetail> shoppingCart = GetShoppingCart();
            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                TempData[ERROR_MESSAGE] = "Không thể tạo đơn hàng với giỏ hàng trống";
                return RedirectToAction("Create");
            }

            if (customerId == 0 || employeeId == 0)
            {
                TempData[ERROR_MESSAGE] = "Vui lòng chọn khách hàng và nhân viên phụ trách";
                return RedirectToAction("Create");
            }

            int orderID = OrderDataService.InitOrder(customerId, employeeId, DateTime.Now, shoppingCart);

            HttpContext.Session.Remove("SHOPPING_CART");
            ClearCart();
            return RedirectToAction("Details", new { id = orderID });
        }
    }
}
