using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string CATEGORY_SEARCH = "category_search";
        public IActionResult Index()
        {
            /*int rowCount = 0;
            var data = CommonDataService.ListOfCategories(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            // tạo ra 1 đối tượng để truyền (cách model)
            var model = new Models.CategorySearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);*/

            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Loại hàng";
            Category model = new Category()
            {
                CategoryId = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Loại hàng";
            Category? model = CommonDataService.GetCategory(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Category data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.CategoryId == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
                if (string.IsNullOrWhiteSpace(data.CategoryName))
                    ModelState.AddModelError(nameof(data.CategoryName), "Tên không được để trống"); // nameof (lấy ra cái tên ứng với data.CustomerName => "CustomerName")
                if (string.IsNullOrEmpty(data.Description))
                    ModelState.AddModelError(nameof(data.Description), "Mô tả không được để trống");
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\categories"); //@"D:\LTWEB\SV20T1020313\SV20T1020313.Web\wwwroot\images\employees";//đường dẫn đến thư mục lưu file
                    string filePath = Path.Combine(folder, fileName); // đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.CategoryId == 0)
                {
                    int id = CommonDataService.AddCategory(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Đã xảy ra lỗi khi thêm hàng, Vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCategory(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Đã xảy ra lỗi khi thêm hàng, Vui lòng thử lại sau");
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
        public ActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCategory(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUsedCategory(id);
            return View(model);
        }
    }
}
