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
                    Text = "-- Chon tỉnh/thanh --"});

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
    }
}
