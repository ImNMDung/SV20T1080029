using SV20T1080029.Web.Models;
using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;


namespace SV20T1080029.Web.Areas.Admin.Controllers
{/// <summary>
 /// 
 /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class CustomerController : Controller
    {
        public int PAGE_SIZE = 10;
        private const string CUSTOMER_SREACH = "Customer_Search";
        //public IActionResult Index(int page = 1 , string searchValue = "")

        //{
        //    int rowCount  = 0;
        //    //  var model = CommonDataService.ListOfCustomer(out rowCount, page, 10, searchValue);
        //    var data  = CommonDataService.ListOfCustomers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
        //    var model = new PaginationSearchCustomer() 
        //    {
        //        Page = page,
        //        PageSize = PAGE_SIZE,
        //        SearchValue = searchValue ?? "",
        //        Data = data,
        //        RowCount = rowCount,
        //    };
        //  //  string errorMessage = TempData["ErrorMessage"]?.ToString() ?? "Giá trị mặc định nếu TempData là null";

        //    string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
        //    ViewBag.ErrorMessage = errorMessage;

        //    string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
        //    ViewBag.SuccessMessage = successMessage;

        //    string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
        //    ViewBag.AddSuccessMessage = addsuccessMessage;

        //    ViewBag.ErrorMessage = errorMessage;
        //    return View(model); ;
        //}
        public IActionResult Index()

        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SREACH);

            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,

                    PageSize = PAGE_SIZE,

                    SearchValue = "",

                    

                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {

            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new PaginationSearchCustomer()
            {
                Page = input.Page,
                PageSize = input.PageSize,

                SearchValue = input.SearchValue ?? "",
                Data = data,
                RowCount = rowCount,
            };
            ApplicationContext.SetSessionData(CUSTOMER_SREACH, input); // lưu lại điều kiêịn tìm kieem
            return View(model);

        }
        public IActionResult Create()
        {
            ViewBag.title = "Bổ Sung Khách Hàng";

            var model = new Customer()
            {
                CustomerID = 0
            };


            return View(model);
        }
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetCustomer(id);
            ViewBag.title = "Sửa Khách Hàng";
            return View("Create", model);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCustomer(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCustomer(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Changepass(int id = 0)
        {
            ViewBag.title = "Đổi Mật Khẩu";
            return View("Create");
        }

        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật khách hàng";
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được rỗng");

            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError(nameof(data.Address), "Address không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Phone không được rỗng");

            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Province không được rỗng");
            if (!ModelState.IsValid)
            {
                return View("Create", data);
            }
            if (data.CustomerID == 0)
            {
                int customerId = CommonDataService.AddCustomer(data);
                if (customerId > 0)
                {
                    TempData["AddSuccessMessage"] = "Bổ sung thành công !";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Không bổ sung được dữ liệu";
                    return View("Create", data);
                }
            }
            else
            {
                if (data != null)
                {
                    CommonDataService.UpdateCustomer(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }

    }
}
