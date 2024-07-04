using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SV20T1020313.Web.Models;
using System.Globalization;

namespace SV20T1020313.Web.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Create()
        {
            var model = new Models.Person()
            {
                Name = "Test",
                BirthDate = DateTime.Now,
                Salary = 10.25m
            };
            return View(model);
        }
        public IActionResult Save(Models.Person model, string birthDateInput = "")
        {
            DateTime? dValue = Converters.ToDateTime(birthDateInput);
            if (dValue.HasValue)
            {
                model.BirthDate = dValue.Value;
            }
            return Json(model);
        }
        
    }
}
