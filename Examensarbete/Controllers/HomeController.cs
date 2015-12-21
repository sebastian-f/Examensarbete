using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service.Interface;

namespace Examensarbete.Controllers
{
    public class HomeController : Controller
    {
        ICategoryService categoryService;
        public HomeController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Rooms()
        {
            IList<Service.DTO.CategoryModel> categories = categoryService.GetAllCategories().ToList();
            return View(categories);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult AllRooms()
        {
            return PartialView("RoomsPartial",categoryService.GetAllCategories());
        }
    }
}