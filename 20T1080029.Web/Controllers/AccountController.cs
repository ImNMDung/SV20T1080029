using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1080029.BusinessLayers;

namespace SV20T1080029.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
      public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string userName ="", string password ="")
        {
           
            ViewBag.UserName = userName;
            ViewBag.Password = password;
            var userAccount = UserAccountService.Authorize(userName, password,TypeOfAccount.Employee);

            // kiểm tra thông tin đăng nhập
            //TODO : kiểm tra username và password từ csdl

            if(userAccount !=null)
            {
                // đăng nhập thành công
                // tạo đối tượng lưu các thông tin phiên đăng nhập
                WebUserData userData = new WebUserData()
                {
                    UserId = userAccount.UserId,
                    UserName = userAccount.UserName,
                    DisplayName = userAccount.FullName,
                    Email = userAccount.Email,
                    Photo = userAccount.Photo,
                    ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    SessionId = HttpContext.Session.Id,
                    AdditionalData = "",
                    Roles = new List<string>() { WebUserRoles.Administrator}


                };
                //thiết lập (ghi nhận) phiên đăng nhập
                await HttpContext.SignInAsync(userData.CreatePrincipal());
                // quay lại đăng chủ admin
                return RedirectToAction("Index", "Dashboard",new {area ="Admin"} );
                


            }   
            else
            {
                //đăng nhập ko thành công trả lại giao diện đăng nhập
                ModelState.AddModelError("Error", "Đăng nhập không thành công");
                return View();
            }    

        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");

        }
        // đổi mk
        [Authorize]


        [HttpGet]
       
        public IActionResult ChangePassword ()
        {
            return View();
           
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            string userName = User.GetUserData().UserName;
            // kiểm tra mật khẩu đúng chưa
            var userAccount = UserAccountService.Authorize(userName, oldPassword, TypeOfAccount.Employee);
            if (userAccount == null)
                ModelState.AddModelError("Error", "Mật khẩu không đúng");
            // kiểm tra có trùng mật khẩu cũ không ?
            if (newPassword == oldPassword)
                ModelState.AddModelError("Error", "Mật khẩu trùng với mật khẩu cũ");
            // kiểm tra mật khẩu nhập lại có trùng với mật khẩu mới hay không
            if (confirmNewPassword != newPassword)
                ModelState.AddModelError("Error", "Mật khẩu không trùng với mật khẩu mới");

            // đổi mật khẩu thất bại
            if (!ModelState.IsValid)
                return View();
            // thay đổi mật khẩu thành công

            bool result = UserAccountService.ChangePassword(userName, newPassword, TypeOfAccount.Employee);
            //trả về giao diện doashboard
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

    }

}
