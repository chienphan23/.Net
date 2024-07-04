using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 15;
        private const string EMPLOYEE_SEARCH = "employee_search";
        public IActionResult Index()
        {
            /*int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue ?? "");
            var model = new Models.EmployeeSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            
            return View(model);*/
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0,
                BirthDate = new DateTime(1990,1, 1),
                Photo = "nophoto.png"
            };
            return View("Edit", model);// sử dụng view có tên là Edit
    }
    public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            Employee? model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(model.Photo))
            {
                model.Photo = "nophoto.png";
            }

            return View(model);
        }

        [HttpPost]  
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Tên không được để trống"); 
                if (string.IsNullOrEmpty(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
                if (string.IsNullOrEmpty(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }


                // xử lý ngày sinh
                DateTime? birthDate = birthDateInput.ToDateTime();
                if(birthDate.HasValue)
                {
                    data.BirthDate = birthDate.Value;
                }
                // xử lý ảnh
                    // nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho nhân viên
                if(uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\employees"); //@"D:\LTWEB\SV20T1020313\SV20T1020313.Web\wwwroot\images\employees";//đường dẫn đến thư mục lưu file
                    string filePath = Path.Combine(folder, fileName); // đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.EmployeeID == 0)
                {
                    int id = CommonDataService.AddEmployee(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateEmployee(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng với nhân viên khác");
                        return View("Edit", data);

                    }
                    var user = User.GetUserData();
                    if (user?.UserId == data.EmployeeID.ToString())
                    {
                        WebUserData userData = new WebUserData()
                        {
                            UserId = data.EmployeeID.ToString(),
                            UserName = data.Email,
                            DisplayName = data.FullName,
                            Email = data.Email,
                            Photo = data.Photo,
                            ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            SessionId = HttpContext.Session.Id,
                            AdditionalData = "",
                            Roles = data.RoleNames.Split(',').ToList()
                        };
                        HttpContext.SignInAsync(userData.CreatePrincipal());
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
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(model);
        }

        public IActionResult ChangePassword(int id = 0)
        {
            var data = CommonDataService.GetEmployee(id);
            return View(data);

        }
        public IActionResult SavePassword(string UserName = "", string OldPassword = "", string NewPassword = "", int id = 0)
        {
            bool result = false;
            var data = CommonDataService.GetEmployee(id);
            if (string.IsNullOrWhiteSpace(OldPassword))
                ModelState.AddModelError("OldPassword", "Vui lòng nhập mật khẩu cũ");
            if (string.IsNullOrWhiteSpace(NewPassword))
                ModelState.AddModelError("NewPassword", "Vui lòng nhập mật khẩu mới");
            
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                result = UserAccountService.ChangePassword(UserName, OldPassword, NewPassword);
            }

            if (!result)
            {
                ModelState.AddModelError("Result", "Đổi mật khẩu không thành công");
            }
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", data);
            }
            return View("Edit", data);
        }
    }
}
