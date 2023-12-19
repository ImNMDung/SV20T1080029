using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web.AppCodes;
using SV20T1080029.Web.Models;
using System.Drawing.Printing;
using System.Reflection;

namespace SV20T1080029.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 6;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchEmployee()
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
       
        public IActionResult Create()
        {
            var model = new Employee()
            {
                
                EmployeeId = 0
            };
            ViewBag.Title = "Thêm nhân viên";
            return View(model);
        }

        public IActionResult Edit(int id = 0)
        {
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật nhân viên";
            return View("Create", model);
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = CommonDataService.DeleteEmployee(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhân viên này !";
                }
                else
                {
                    TempData["SuccessMessage"] = "Xóa thành công !";
                }
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Changepass()
        {
            ViewBag.title = "Đổi mật khẩu";
            return View();
        }
        public IActionResult Save(Employee data , string birthday, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeId == 0 ? "Bổ sung nhân viên" : "Cập nhật nhân viên";
            //Xử lý ngày sinh
            DateTime? dBirthDate = Converter.StringToDateTime(birthday);
            if (dBirthDate == null)
                ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh không hợp lệ");
            else
                data.BirthDate = dBirthDate.Value;

            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\employees", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            //Kiểm tra đầu vào của model

            if (!ModelState.IsValid)
                return Content("Có lỗi xảy ra");
           // return Json(data);
            //Lưu dữ liệu (lưu model vào database)
            //return RedirectToAction("Index");
            if (data.EmployeeId == 0)
            {
                int employeeId = CommonDataService.AddEmployee(data);
                if (employeeId > 0)
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
                    CommonDataService.UpdateEmployee(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }
    }
}
