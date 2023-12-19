using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web;
using SV20T1080029.Web.Models;

using System.Reflection;

namespace LiteCommerce.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    /// Authentication - xac thuc 
    /// Authoiation -   OWin - 
    /// -- BIên session 
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 9;

        private const string PRODUCTS_SREACH = "Products_Search";
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0)
        {
           // int rowCount = 0;

            var input = ApplicationContext.GetSessionData<PaginationSearchProductInput>(PRODUCTS_SREACH);

            if (input == null)
            {
                input = new PaginationSearchProductInput()
                {
                    Page = 1,

                    PageSize = PAGE_SIZE,

                    SearchValue = "",

                    categoryID = categoryID,

                    supplierID = supplierID


                };
            }
            string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;

            string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            ViewBag.SuccessMessage = successMessage;

            string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            ViewBag.AddSuccessMessage = addsuccessMessage;
            return View(input);




            //var data = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID, out rowCount);
            ////  var data1 = CommonDataService.ListOfCategoryNames();
            ////  var data2 = CommonDataService.ListOfSupplierNames();




            //var model = new PaginationSearchProductOutput()
            //{
            //    Page = page,
            //    PageSize = PAGE_SIZE,
            //    SearchValue = searchValue ?? "",
            //    RowCount = rowCount,
            //    Data = data,
            //    CategoryID = categoryID,
            //    SupplierID = supplierID

            //};

            //string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            //ViewBag.ErrorMessage = errorMessage;

            //string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            //ViewBag.SuccessMessage = successMessage;

            //string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            //ViewBag.AddSuccessMessage = addsuccessMessage;
            //return View(model);
        }

        public IActionResult Search(PaginationSearchProductInput input)
        {

            int rowCount = 0;

           // var data = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID, out rowCount);

            var data = ProductDataService.ListProducts( input.Page, input.PageSize, input.SearchValue ?? "",input.categoryID , input.supplierID, out rowCount);
           
            var model = new PaginationSearchProductOutput()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CategoryID = input.categoryID,
                SupplierID = input.supplierID

            };
            ApplicationContext.SetSessionData(PRODUCTS_SREACH, input); // lưu lại điều kiêịn tìm kieem
            return View(model);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var model = new ViewProduct()
            {
                product = new Product { ProductID = 0 }, // Assuming ProductID is an int property
                productAttribute = new ProductAttribute { ProductID = 0 }, // Assuming ProductAttributeID is an int property
                productPhoto = new ProductPhoto { ProductID = 0 } // Assuming ProductPhotoID is an int property


            };


            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            var product = ProductDataService.GetProduct(id);
            var photo = ProductDataService.GetPhoto(id);
            var attribute = ProductDataService.GetAttribute(id);

            var model = new ViewProduct()
            {
                product = product,
                productPhoto = photo,
                productAttribute = attribute
            };
 // sum
            if (model == null)
            {
                return RedirectToAction("Index");

            }
            ViewBag.Title = "Cập nhật mặt hàng";
            return View("Create", model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool success = ProductDataService.DeleteProduct(id);
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

            var model = ProductDataService.GetProduct(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult Photo(int id = 0, string method = "add", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    return View();
                case "delete":
                    //TODO: Delete photo
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult Attribute(int id = 0, string method = "add", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    return View();
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    return View();
                case "delete":
                    //TODO: Delete Attribute
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Save(Product data, IFormFile? uploadPhoto, string ss)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
            //Xử lý ngày sinh


            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Product
            if (uploadPhoto != null)
            {
                //  string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";

                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }


            //Kiểm tra đầu vào của model

            if (!ModelState.IsValid)
                return Content("Có lỗi xảy ra");
             return Json(data);
            //     Lưu dữ liệu(lưu model vào database)
            // return RedirectToAction("Index");
            if (data.ProductID == 0)
            {
                int productsid = ProductDataService.AddProduct(data);
                if (productsid > 0)
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
                    ProductDataService.UpdateProduct(data);
                    TempData["SuccessMessage"] = "Cập nhật thành công !";
                }

                return RedirectToAction("Index");

            }
        }


        [HttpPost]
        public IActionResult DelPhotoinRoot(Product data, IFormFile? uploadPhoto, string file, Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingenvironment)
        {

            string fileDirectory = Path.Combine(
                     Directory.GetCurrentDirectory(), "wwwroot/images/products/");
            ViewBag.fileList = Directory
                .EnumerateFiles(fileDirectory, "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);
            ViewBag.fileDirectory = fileDirectory;
            string webRootPath = _hostingenvironment.WebRootPath;
            var fileName = "";
            fileName = file;
            var fullPath = webRootPath + "/images/products/" + file;

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                ViewBag.deleteSuccess = "true";
            }

            return View();
        }
    }
}