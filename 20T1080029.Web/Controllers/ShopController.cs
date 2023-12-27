using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;
using SV20T1080029.DomainModels;
using SV20T1080029.Web.Models;
using System.Reflection;

namespace SV20T1080029.Web.Controllers
{
    public class ShopController : Controller
    {
        private const int PAGE_SIZE = 9;
        public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0, int minPrice = 0, int maxPrince = 0)
        {
            int rowCount = 0;






            var data = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID, minPrice, maxPrince, out rowCount);
            //  var data1 = CommonDataService.ListOfCategoryNames();
            //  var data2 = CommonDataService.ListOfSupplierNames();




            var model = new PaginationSearchProductOutput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data,
                CategoryId = categoryID,
                SupplierId = supplierID

            };

            var doubleView = new DoubleView()
            {
                data1 = model,
             
            };


            return View(doubleView);
        }

        public IActionResult Details(int id, int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0, int minPrice = 0, int maxPrince = 0)
        {
            int rowCount = 0;

            // Retrieve list of products for pagination
            var listProducts = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID,  minPrice ,  maxPrince , out rowCount);

            // Retrieve details of the specific product
            var productDetails = ProductDataService.GetProduct(id);

            // Check if either the list of products or the specific product is null
            if (listProducts == null || productDetails == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PaginationSearchProductOutput()
            {
               
                    Page = page,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue ?? "",
                    RowCount = rowCount,
                    Data = listProducts,
                    CategoryId = categoryID,
                    SupplierId = supplierID
             
              
            };
            var doubleView = new DoubleView()
            {
                data1 = model,
                data2 = productDetails
            };

           
                return View(doubleView);
        }



    }
}
