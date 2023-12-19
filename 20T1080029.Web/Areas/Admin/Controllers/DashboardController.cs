using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}
