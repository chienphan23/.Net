using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SHIPPER_SEARCH = "shipper_search";
        public IActionResult Index()
        {
            /*int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            // tạo ra 1 đối tượng để truyền (cách model)
            var model = new Models.ShipperSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);*/

            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Người giao hàng";
            Shipper model = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            Shipper? model = CommonDataService.GetShipper(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            try
            {
                ViewBag.Title = data.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhật thông tin người giao hàng";

                if (string.IsNullOrWhiteSpace(data.ShipperName))
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên không được để trống");
                if (string.IsNullOrEmpty(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");

                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Thêm người giao hàng thất bại vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Thêm nhà cung cấp thất bại vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUsedShipper(id); // cho phép xoá trong trưởng hợp không được sử dụng (chặn nút delete)
            return View(model);
        }
    }
}
