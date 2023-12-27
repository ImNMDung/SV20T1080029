using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web.Models;

namespace SV20T1080029.Web.Areas.Admin.Controllers
{/// <summary>
 /// 
 /// </summary>
 /// 
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 10;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCategorys(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchCategory()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;

            string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            ViewBag.SuccessMessage = successMessage;

            string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            ViewBag.AddSuccessMessage = addsuccessMessage;
            return View(model);
        }

        // create loại hàng
        public IActionResult Create()
        {
            var model = new Category()
            {
                CategoryID = 0
            };
            ViewBag.Title = "Bổ sung loại hàng";
            return View(model);
        }
        // Edit loại hàng
        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                Console.WriteLine($"Error in Get method: {model}");
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật loại hàng";
            return View("Create", model);
        }
        // delete loại hàng
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteCategory(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa loại hàng này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCategory(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        /// <summary>
        /// lưu dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult Save(Category data)
        {
          
          
           

            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung khách hàng" : "Cập nhật khách hàng";

            if (data.CategoryID == 0)
            {
                int categoryId = CommonDataService.AddCategory(data);
                if (categoryId > 0)
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
                    CommonDataService.UpdateCategory(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }
    }
}
