using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize (Roles = $"{WebUserRoles.Employee},{WebUserRoles.Administrator}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SUPPLIER_SEARCH = "supplier_search";
        public IActionResult Index()
        {
            /*int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            // tạo ra 1 đối tượng để truyền (cách model)
            var model = new Models.SupplierSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);*/

            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
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
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Nhà cung cấp";
            Supplier model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", model);// sử dụng view có tên là Edit
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Nhà cung cấp";
            Supplier? model = CommonDataService.GetSupplier(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);// không cần chỉ định, mặc định là view có cùng tên với action
        }
        [HttpPost]
        public IActionResult Save(Supplier data)
        {
            try
            {
                ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";

                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên không được để trống"); 
                if (string.IsNullOrEmpty(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrEmpty(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập Số điện thoại của nhà cung cấp");
                if (string.IsNullOrEmpty(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của nhà cung cấp");
                if (string.IsNullOrEmpty(data.Provice))
                    ModelState.AddModelError(nameof(data.Provice), "Vui lòng chọn tỉnh thành");

                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }
                
                if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Thêm nhà cung cấp thất bại vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                else
                {   
                    bool result = CommonDataService.UpdateSupplier(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Thêm nhà cung cấp thất bại vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.isUsedSupplier(id); // cho phép xoá trong trưởng hợp không được sử dụng (chặn nút delete)
            return View(model);
        }
    }
}
