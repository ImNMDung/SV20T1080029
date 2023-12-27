using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;
using SV20T1080029.Web.Models;

namespace SV20T1080029.Web.Areas.Admin.Controllers
{/// <summary>
 /// 
 /// </summary>
    [Area("Admin")]
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]// chuyển đến đăng nhập
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {

            var countCustomer = CommonDataService.CountCustomer();

            var countEmployee = CommonDataService.CountEmployeer();

            var countProduct = ProductDataService.CountProduct();

            var countOrder = OrderDataService.CountOrder();


            var viewModel = new ViewDashboard { 
                CountCustomer = countCustomer,
                CountEmployee =countEmployee,
                CountOrder = countOrder,
                CountProduct = countProduct

            };
            return View(viewModel);


           
        }
    }
}
