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
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    /// <summary>
    /// 
    /// </summary>
    [Area("Admin")]
    public class ProductController : Controller
    {
        private const string Product_Search = "Product_Search";

        private const string Product_Data_Photo = "Product_Data_Photo";

        private const string Product_Data_Attribute = "Product_Data_Attribute";

        public const int Page_Size = 10; // Tạo một biến hằng để đồng bộ thuộc tính cho trang web.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchProductInput>(Product_Search);
            if (input == null)
            {
                input = new PaginationSearchProductInput()
                {
                    Page = 1,
                    PageSize = Page_Size,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MaxPrice = 0,
                    MinPrice = 0,
                };
            }

            return View(input);
        }

        /// <summary>
        /// Hàm trả về danh sách tìm kiếm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchProductInput input)
        {

            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice);
            var model = new PaginationSearchProductOutput()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CategoryId = input.CategoryID,

                SupplierId = input.SupplierID,

                //MinPrice = input.MinPrice,
                //MaxPrice = input.MaxPrice
            };
            ApplicationContext.SetSessionData(Product_Search, input);//lưu lại điều kiện tìm kiếm

            string errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;
            string deletedMessage = Convert.ToString(TempData["DeletedMessage"]);
            ViewBag.DeletedMessage = deletedMessage;
            string savedMessage = Convert.ToString(TempData["SavedMessage"]);
            ViewBag.SavedMessage = savedMessage;

            return View(model);
        }

        /// <summary>
        /// Bổ sung một mặt hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var product = new Product()
            {
                ProductId = 0
            };
            
           


            var data = new ViewProduct()
            {
                Product = product,
                ProductAttributes = null,
                ProductPhotos = null
            };

            ApplicationContext.SetSessionData(Product_Data_Photo, data);
            return View(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            var product = ProductDataService.GetProduct(id);
            var listAttributes = ProductDataService.ListAttributes(id);
            var listPhotos = ProductDataService.ListPhotos(id);
            if (product == null || listAttributes == null || listPhotos == null)
            {
                return RedirectToAction("Index");
            }
            var data = new ViewProduct
            {
                Product = product,
                ProductAttributes = listAttributes,
                ProductPhotos = listPhotos
            };
            ViewBag.Title = "Cập nhật mặt hàng";
            return View(data);
        }

        public IActionResult Save(ViewProduct data, IFormFile? uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(data.Product.ProductName))
            {
                ModelState.AddModelError(nameof(data.Product.ProductName), "* Tên mặt hàng không được để trống!");
            }
            if (data.Product.CategoryId == 0)
            {
                ModelState.AddModelError(nameof(data.Product.CategoryId), "* Vui lòng chọn loại hàng!");
            }
            if (data.Product.SupplierId == 0)
            {
                ModelState.AddModelError(nameof(data.Product.SupplierId), "* Vui lòng chọn nhà cung cấp!");
            }
            if (string.IsNullOrWhiteSpace(data.Product.Unit))
            {
                ModelState.AddModelError(nameof(data.Product.Unit), "* Đơn vị của mặt hàng không được để trống!");
            }
            if (data.Product.Price == 0)
            {
                ModelState.AddModelError(nameof(data.Product.Price), "* Vui lòng nhập giá sản phẩm!");
            }

            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Product.Photo = fileName;
                //model.Photo = fileName;

            }

            if (ModelState.IsValid == false)
            {
                if (data.Product.ProductId == 0)
                    return View("Create", data);
                if (data.Product.ProductId > 0)
                    return View("Edit", data);
            }


            if (data.Product.ProductId == 0)
            {
                int productId = ProductDataService.AddProduct(data.Product);
                if (productId > 0)
                {
                    TempData["SavedMessage"] = "Thông tin mặt hàng đã được lưu lại!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Không bổ sung được dữ liệu!";
                    return View("Create", data);
                }
            }
            else
            {
                bool editSuccess = ProductDataService.UpdateProduct(data.Product);
                if (editSuccess)
                {
                    TempData["SavedMessage"] = "Thông tin mặt hàng đã được chỉnh sửa!";

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Chỉnh sửa thông tin mặt hàng không thành công!";
                    return View("Edit", data);
                }
            }
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
                    TempData["ErrorMessage"] = "Không được phép xóa mặt hàng này này";
                }
                else
                {
                    TempData["DeletedMessage"] = "Xóa mặt hàng thành công!";
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
        /// Quản lí ảnh của mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="photoId"></param>
        /// <returns></returns>
        public IActionResult Photo(int id = 0, string method = "add", long PhotoId = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            ProductPhoto data = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    data = new ProductPhoto()
                    {
                        PhotoId = 0,
                        ProductId = id
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    if (PhotoId < 0)
                    {
                        return RedirectToAction("Index");
                    }
                    data = ProductDataService.GetPhoto(PhotoId);
                    if (data == null)
                    {
                        return RedirectToAction("index");
                    }
                    return View(data);
                case "delete":
                    ProductDataService.DeletePhoto(PhotoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Lưu ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <param name="uploadPhoto"></param>
        /// <returns></returns>
        public ActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            // kiểm tra dữ liệu đầu vào
            //if (data.PhotoID == 0 && uploadPhoto == null) {
            //    ModelState.AddModelError(nameof(data.Photo), "Vui lòng thêm hình ảnh");
            //}
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh không được để trống");
            }
            else if (data.DisplayOrder < 1)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị hình ảnh phải là một số tự nhiên dương");
            }
             
            List<ProductPhoto> productPhotos = ProductDataService.ListPhotos(data.ProductId);
            bool isUsedDisplayOrder = false;
            foreach (ProductPhoto item in productPhotos)
            {
                if (item.DisplayOrder == data.DisplayOrder && data.PhotoId != item.PhotoId)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError("DisplayOrder",
                    $"Thứ tự hiển thị {data.DisplayOrder} của hình ảnh đã được sử dụng trước đó");
            }

            data.Description = data.Description ?? "";
            data.IsHidden = Convert.ToBoolean(data.IsHidden.ToString());
            //Xử lý với ảnh
            //Upload ảnh lên (nếu có), sau khi upload xong thì mới lấy tên file ảnh vừa upload
            //để gán cho trường Photo của Employee
            if (uploadPhoto != null)
            {
                string fileName = $"{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products\productPhotos", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
                //model.Photo = fileName;

            }
            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.PhotoId == 0 ? "Bổ sung ảnh" : "Thay đổi ảnh";
                return View("Photo", data);
            }

            // thực hiện thêm hoặc cập nhật
            if (data.PhotoId == 0)
            {
                ProductDataService.AddPhoto(data);
            }
            else
            {
                ProductDataService.UpdatePhoto(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductId });
        }

        /// <summary>
        /// Các trường hợp chỉnh sửa thuộc tính của mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public IActionResult Attribute(int id = 0, string method = "add", int attributeId = 0)
        {
            if (id < 0)
            {
                return RedirectToAction("Index");
            }
            
            ProductAttribute data = null;
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    data = new ProductAttribute()
                    {
                        AttributeId = 0,
                        ProductId = id,
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    if (attributeId < 0)
                    {
                        return RedirectToAction("Index");
                    }
                    data = ProductDataService.GetAttribute(attributeId);
                    if (data == null)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(data);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id }); //return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Lưu dữ liệu của thuộc tính mặt hàng
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveAttribute(ProductAttribute data)
        {
            // kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.AttributeName))
            {
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            }
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
            {
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            }

            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính không được để trống");
            }
            else if (data.DisplayOrder < 1)
            {
                ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị thuộc tính phải > 0");
            }
            List<ProductAttribute> productAttributes = ProductDataService.ListAttributes(data.ProductId);
            bool isUsedDisplayOrder = false;
            foreach (ProductAttribute item in productAttributes)
            {
                if (item.DisplayOrder == data.DisplayOrder && data.AttributeId != item.AttributeId)
                {
                    isUsedDisplayOrder = true;
                    break;
                }
            }
            if (isUsedDisplayOrder)
            {
                ModelState.AddModelError("DisplayOrder",
                        $"Thứ tự hiển thị {data.DisplayOrder} của thuộc tính đã được sử dụng trước đó");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.AttributeId == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", data);
            }

            // thực hiện thêm hoặc cập nhật
            if (data.AttributeId == 0)
            {
                ProductDataService.AddAttribute(data);
            }
            else
            {
                ProductDataService.UpdateAttribute(data);
            }
            return RedirectToAction("Edit", new { id = data.ProductId });
        }
    }
}