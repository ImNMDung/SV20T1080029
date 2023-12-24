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

namespace SV20T1080029.Web.Areas.Admin.Controllers
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

            var data = ProductDataService.ListProducts(input.Page, input.PageSize, input.SearchValue ?? "", input.categoryID, input.supplierID,input.minPrice , input.maxPrice , out rowCount);
       
            var model = new PaginationSearchProductOutput()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                categoryID = input.categoryID,

                supplierID = input.supplierID,

                minPrice = input.minPrice,
                maxPrice = input.maxPrice
            };
            ApplicationContext.SetSessionData(PRODUCTS_SREACH, input); // lưu lại điều kiêịn tìm kieem
                
            var productinput = new ViewProduct
            {
                data1 = model,
            };


            return View(productinput);

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
                product = new Product { ProductId = 0 }, // Assuming ProductID is an int property
                productPhotos = new List<ProductPhoto>(),
                productAttributes = new List<ProductAttribute>()
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
            ViewBag.Title = "Cập nhật mặt hàng";
            //if (id > 0)
            //{
            //    return RedirectToAction("Index");
            //}
            Product product = ProductDataService.GetProduct(id);
            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(id);
            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(id);
            if (product == null || productAttributes == null || productPhotos == null)
            {
                return RedirectToAction("Index");
            }
            var model = new ViewProduct()
            {
                product = product,
                productPhotos = productPhotos,
                productAttributes = productAttributes
            };
            // sum
            if (model == null)
            {
                return RedirectToAction("Index");

            }
           
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
        

        public IActionResult Save(Product data, IFormFile? uploadPhoto, string ss )
        {
            ViewBag.Title = data.ProductId == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
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
            if (data.ProductId == 0)
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

        public ActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            // kiểm tra dữ liệu đầu vào
            // Check input data (commented out code)
            // It seems to be checking if both PhotoID is 0 and uploadPhoto is null, then add a ModelState error.

            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(data.ProductId);
            bool isUsedDisplayOrder = false;

            // Check if the DisplayOrder is already used by another photo
            foreach (ProductPhoto item in productPhotos)
            {
                if (item.DisplayOrder == data.DisplayOrder && data.PhotoId != item.PhotoId)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }

            // Set default values for Description and IsHidden if they are null
            data.Description = data.Description ?? "";
            data.IsHidden = Convert.ToBoolean(data.IsHidden.ToString());

            // xử lý nghiệp vụ upload file (handle file upload business logic)
            if (uploadPhoto != null)
            {
                // Generate a unique filename using the current timestamp and the original filename
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);

                // Save the uploaded file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }

                // Set the Photo property of the data object to the generated filename
                data.Photo = fileName;
            }
            return Json(data);
            // thực hiện thêm hoặc cập nhật (perform add or update)
            if (data.PhotoId == 0)
            {
                // Add a new photo
                ProductDataService.AddPhoto(data);
            }
            else
            {
                // Update an existing photo
                ProductDataService.UpdatePhoto(data);
            }

            // Redirect to the Edit action for the associated product
            return RedirectToAction($"Edit/{data.ProductId}");
        }
    }
   

}