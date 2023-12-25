using SV20T1080029.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;

namespace SV20T1080029.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //truy cập Index sẽ lấy danh sanh 3 người vừa làm trong peson - lấy viewww
            // C1 view data  ( disconnalri

            /*
                        var dal = new PersonDAL();
                        var data = dal.List();
                        ViewData["LitleMessage"] = "List Of Person";
                        ViewData["ListOfPerson"] = data;*/


            //C2 dynamic Oject



            //var dal = new PersonDAL();
            //var data = dal.List();
            //ViewBag.TitleMessage = " List of Person";
            //ViewBag.ListOfPersons = data;

            //C3 
            //var dal = new PersonDAL();
            //var data = dal.List();
            //ViewData["LitleMessage"] = "List Of Person";
            var data = new HomeIndexModel()
            {
                   TitleMessage = "List of Persons and Students",
                     ListOfPersons = new PersonDAL().List(),
                    ListOfStudents = new StudentDal().List()

        };



            return RedirectToAction("Order","Admin");
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


        public IActionResult GetEmployee(InputEmployee data)
        {
            //string s = "";
            
            //foreach (string n in name)
            //{
            //    s += n;
            //}
            return Content($"name: {data.Name}, age: {data.Age} ,addres {data.Address}");
        }
        public IActionResult InputEmployee()
        {
            return View();
        }
    }
}