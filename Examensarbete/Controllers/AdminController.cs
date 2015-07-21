using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;

namespace Examensarbete.Controllers
{
    public class AdminController : Controller
    {
        private ICategoryRepository categoryRepository;        
        public AdminController(ICategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;        
        }

        public ActionResult Index()
        {
            return View(categoryRepository.Categories);
        }
    }
}