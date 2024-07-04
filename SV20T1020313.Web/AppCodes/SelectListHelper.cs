using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020313.BusinessLayers;

namespace SV20T1020313.Web
{
    public class SelectListHelper
    {
        public static List<SelectListItem> Province()   // selectlistitem để tạo ra các cái select and options
        {
            List<SelectListItem> list = new List<SelectListItem> ();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn Tỉnh/thành --"
            });
            foreach (var item in CommonDataService.ListOfProvinces()) 
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }

        public static List<SelectListItem> Supplier()   // selectlistitem để tạo ra các cái select and options
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn Nhà cung cấp --"
            });
            foreach (var item in CommonDataService.ListOfSuppliers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        public static List<SelectListItem> Category()   // selectlistitem để tạo ra các cái select and options
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn loại hàng --"
            });
            foreach (var item in CommonDataService.ListOfCategories())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryId.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List<SelectListItem> Customer()   // selectlistitem để tạo ra các cái select and options
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn Khách hàng --"
            });
            foreach (var item in CommonDataService.ListOfCustomers())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName
                });
            }
            return list;
        }
    }
}
