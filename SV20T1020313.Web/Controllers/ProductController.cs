using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020313.BusinessLayers;
using SV20T1020313.DomainModels;
using SV20T1020313.Web.Models;

namespace SV20T1020313.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Employee},{WebUserRoles.Administrator}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH = "product_search";
        public IActionResult Index()
        {
            /*int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID, minPrice, maxPrice);
            // tạo ra 1 đối tượng để truyền (cách model)
            ViewBag.CategoryID = categoryID;
            ViewBag.SupplierID = supplierID;
            ViewBag.SearchValue = searchValue;
            var model = new Models.ProductSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);*/
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0
                };
            }

            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, 
                                        input.SearchValue ?? "", input.CategoryID, input.SupplierID);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung Mặt hàng";
            ViewBag.IsEdit = false;
            Product model = new Product()
            {
                ProductID = 0
            };
            var model1 = new Models.ProductAttributeSearchResult()
            {
                data = model
            };
            return View("Edit", model1);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin Mặt hàng";
            ViewBag.IsEdit = true;// viết tạm
            Product? model1 = ProductDataService.GetProduct(id);
            
            if (model1 == null)
            {
                return RedirectToAction("Index");
            }
            List<ProductPhoto>? ListDataPhoto = ProductDataService.ListOfPhotos(id);
            List<ProductAttribute>? ListDataAttribute = ProductDataService.ListAttributes(id);
            var model = new Models.ProductAttributeSearchResult()
            {
                data = model1,
                ListDataPhoto = ListDataPhoto,
                ListDataAttribute = ListDataAttribute
            };
            return View(model);// không cần chỉ định, mặc định là view có cùng tên với action
        }

        [HttpPost]  // chỉ nhận giữ liệu gửi lên dưới dạng post
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
                ViewBag.IsEdit = data.ProductID == 0 ? false : true;

                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError(nameof(data.ProductName), "Tên không được để trống"); // nameof (lấy ra cái tên ứng với data.CustomerName => "CustomerName")
                if (string.IsNullOrEmpty(data.ProductDescription))
                    ModelState.AddModelError(nameof(data.ProductDescription), "Mô tả không được để trống");
                if (string.IsNullOrWhiteSpace(data.CategoryID.ToString()))
                    ModelState.AddModelError("Category", "Vui lòng chọn loại hàng");
                if (string.IsNullOrWhiteSpace(data.SupplierID.ToString()))
                    ModelState.AddModelError("Supplier", "Vui lòng chọn nhà cung cấp");
                if (data.CategoryID == 0)
                    ModelState.AddModelError("Category", "Vui lòng chọn loại hàng");
                if (data.SupplierID == 0)
                    ModelState.AddModelError("Supplier", "Vui lòng chọn nhà cung cấp");
                if (string.IsNullOrEmpty(data.Unit))
                    ModelState.AddModelError(nameof(data.Unit), "Vui nhập đơn vị tính");

                if (!ModelState.IsValid)
                {
                    if(data.ProductID == 0)
                    {

                    var model1 = new Models.ProductAttributeSearchResult()
                    {
                        data = data
                    };
                    
                    return View("Edit", model1);
                    }
                    else
                    {

                        List<ProductPhoto>? ListDataPhoto = ProductDataService.ListOfPhotos(data.ProductID);
                        List<ProductAttribute>? ListDataAttribute = ProductDataService.ListAttributes(data.ProductID);
                        var model2 = new Models.ProductAttributeSearchResult()
                        {
                            data = data,
                            ListDataPhoto = ListDataPhoto,
                            ListDataAttribute = ListDataAttribute
                        };
                        return View("Edit", model2);
                    }

                }

                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\products"); //@"D:\LTWEB\SV20T1020313\SV20T1020313.Web\wwwroot\images\employees";//đường dẫn đến thư mục lưu file
                    string filePath = Path.Combine(folder, fileName); // đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                if (data.ProductID == 0)
                {
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError("Error", "Xảy ra lỗi khi thêm mặt hàng, vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                else
                {
                    bool result = ProductDataService.UpdateProduct(data);
                    if (!result)
                    {
                        ModelState.AddModelError("Error", "Xảy ra lỗi khi sửa mặt hàng, vui lòng thử lại sau");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Edit", new {id = data.ProductID});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng thử lại sau vài phút");
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id)
        {
            // nếu truy cập phương thức get => hiển thị thông tin khách hàng
            // nếu truy cập phương thức post => thực hiện xoá khách hàng
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !ProductDataService.isUsedProduct(id); // cho phép xoá trong trưởng hợp không được sử dụng (chặn nút delete)
            return View(model);
        }

        public IActionResult Photo(string id, string method, int photoID = 0)
        {
            switch(method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto modelAdd = new ProductPhoto()
                    {
                        ProductID = int.Parse(id),
                        PhotoId = 0
                    };
                    return View(modelAdd);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh";
                    long convertID = (long)photoID;
                    ProductPhoto? modelEdit = ProductDataService.GetPhoto(convertID);
                    return View(modelEdit);
                case "delete":
                    //ToDo: trực tiếp xoá ảnh (không cần confirm)
                    long convertID1 = (long)photoID;
                    ProductDataService.DeletePhoto(convertID1);
                    return RedirectToAction("Edit", new { id = id });// xoá xong trở lại trang edit
                default: 
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]  // chỉ nhận giữ liệu gửi lên dưới dạng post
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            try
            {
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                    string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\products"); //@"D:\LTWEB\SV20T1020313\SV20T1020313.Web\wwwroot\images\employees";//đường dẫn đến thư mục lưu file
                    string filePath = Path.Combine(folder, fileName); // đường dẫn đến file cần lưu
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }
                if (data.PhotoId == 0)
                {
                    long id = ProductDataService.AddPhoto(data);
                }
                else
                {
                    bool result = ProductDataService.UpdatePhoto(data);
                }
                return RedirectToAction("Edit",new { id = data.ProductID});
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }
        public IActionResult Attribute(string id, string method, int attributeID = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    ProductAttribute modelAdd = new ProductAttribute()
                    {
                        AttributeID = 0,
                        ProductID = int.Parse(id)
                    };
                    return View(modelAdd);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính";
                    long convertID = (long) attributeID;
                    ProductAttribute? modelEdit = ProductDataService.GetAttribute(attributeID);
                    return View(modelEdit);
                case "delete":
                    //ToDo: trực tiếp xoá ảnh (không cần confirm)
                    ProductDataService.DeleteAttribute(attributeID);
                    return RedirectToAction("Edit", new { id = id });// xoá xong trở lại trang edit
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]  // chỉ nhận giữ liệu gửi lên dưới dạng post
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            try
            {
                if (data.AttributeID == 0)
                {
                    long id = ProductDataService.AddAttribute(data);
                }
                else
                {
                    bool result = ProductDataService.UpdateAttribute(data);
                }
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }
      
    }
}
