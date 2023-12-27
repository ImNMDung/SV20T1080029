using SV20T1080029.BusinessLayers;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace SV20T1080029.Web
{
    public class SelectListHelper
    {
        public static List<SelectListItem> Province()
        { 
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
                {
                Value = "",
                    Text = "-- Chon tỉnh/thành --"});

            foreach (var item in CommonDataService.ListOfProvinces())
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName,
                });
            return list;
        }

        public static List<SelectListItem> Category()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn mặt hàng --"
            });

            foreach (var item in CommonDataService.ListOfCategoryNames())
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName,
                });
            return list;
        }


        public static List<SelectListItem> Supplier()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn nhà cung cấp --"
            });

            foreach (var item in CommonDataService.ListOfSupplierNames())
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName,
                });
            return list;
        }
        public static List<SelectListItem> Shippers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "--Chọn người giao hàng--"
            });
            foreach (var item in CommonDataService.ListOfShippers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ShipperID.ToString(),
                    Text = item.ShipperName
                });
            }
            return list;
        }
        public static List<SelectListItem> Status()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Trạng thái --"
            });

            foreach (var item in OrderDataService.ListOfOrders())
                list.Add(new SelectListItem()
                {
                    Value = item.Status.ToString(),
                    Text = item.StatusDescription,
                });
            return list;
        }

        public static List<SelectListItem> Customers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn khách hàng --"
            });

            foreach (var item in CommonDataService.ListOfCustomers())
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName,
                });
            return list;
        }

        public static List<SelectListItem> Employees()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn nhân viên --"
            });

            foreach (var item in CommonDataService.ListOfEmployees())
                list.Add(new SelectListItem()
                {
                    Value = item.EmployeeId.ToString(),
                    Text = item.FullName,
                });
            return list;
        }

    }
}
