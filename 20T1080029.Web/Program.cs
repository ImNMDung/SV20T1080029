using SV20T1080029.Web;
using SV20T1080029.Web.AppCodes;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);
// balanxing - cân bằng tải ;
// In Memory (Process)
// IN server 
// IN database
// Bổ sung các service cần dùng:

// tại lại toàn trang khi cần rếh một phần ọiio dung của trang > ajax + api call 
builder.Services.AddHttpContextAccessor(); // bổ sugn các service cần dùng
builder.Services.AddControllersWithViews() // su dung mvc
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; //Không sử dụng thông báo mặc định cho giá trị null
                                                                                     //// tắt thông báo lỗi mặc định
    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // su dung xac thuc
    .AddCookie(option =>
    {
        option.Cookie.Name = "AuthenticationCookie";
        option.LoginPath = "/Account/Login"; // chưa đăng nhập nó sẽ về trang login
        option.AccessDeniedPath = "/Account/AccessDenied";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);// thời gian đăng nhập 60 phút
    });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapAreaControllerRoute(
        name: "areaAdmin",
        areaName: "Admin",
        pattern: "admin/{controller=Dashboard}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Khởi tạo cấu hình cho ApplicationContext
ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    hostEnvironment: app.Services.GetService<IWebHostEnvironment>()
);

app.Run();