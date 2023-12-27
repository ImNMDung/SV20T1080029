using SV20T1080029.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using SV20T1080029.BusinessLayers;
using System.Drawing.Printing;

namespace SV20T1080029.Web.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 10;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0,int minPrice = 0, int maxPrince = 0)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID,minPrice,maxPrince, out rowCount);


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

            string? errorMessage = Convert.ToString(TempData["ErrorMessage"]);
            ViewBag.ErrorMessage = errorMessage;

            string? successMessage = Convert.ToString(TempData["SuccessMessage"]);
            ViewBag.SuccessMessage = successMessage;

            string? addsuccessMessage = Convert.ToString(TempData["AddSuccessMessage"]);
            ViewBag.AddSuccessMessage = addsuccessMessage;
            var doubleView = new DoubleView()
            {
                data1 = model,

            };


            return View(doubleView);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


       
    }
}