using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize (Roles = $"{WebUserRoles.Administrator}")]    // cho quyền truy cập là customer
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH = "customer_search"; //tên biến dùng để lưu trong session

        public IActionResult Index()
        {
            // lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            // Trường hợp trong session chưa có điều kiện thì tạo điều kiện mới
            if(input == null)
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

        public IActionResult Search (PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount,  input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult() 
            { 
                Page = input.Page,
                PageSize=input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data 
            };

            // Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            Customer model = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit", model);// sử dụng view có tên là Edit
        }
        public IActionResult Edit(int id = 0)
        {   
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Customer? model = CommonDataService.GetCustomer(id);
            if(model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);// không cần chỉ định, mặc định là view có cùng tên với action
        }

        [HttpPost]  // chỉ nhận giữ liệu gửi lên dưới dạng post
        public IActionResult Save(Customer data)
        {
            try
            {
                ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";

                // Kiểm soát đầu vào và đưa các thông báo lỗi vào trong ModelState (nếu có)
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError(nameof(data.CustomerName), "Tên không được để trống"); // nameof (lấy ra cái tên ứng với data.CustomerName => "CustomerName")
                if (string.IsNullOrEmpty(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
                if (string.IsNullOrEmpty(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
                if (string.IsNullOrEmpty(data.Province))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
                // Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    return View("Edit", data);
                }

                if (data.CustomerID == 0)
                {
                    int id = CommonDataService.AddCustomer(data);
                    if(id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = CommonDataService.UpdateCustomer(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng với khách hàng khác");
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
            // nếu truy cập phương thức get => hiển thị thông tin khách hàng
            // nếu truy cập phương thức post => thực hiện xoá khách hàng
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCustomer(id);
            if(model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUsedCustomer(id); // cho phép xoá trong trưởng hợp không được sử dụng (chặn nút delete)
            return View(model);
        }
    }
}
