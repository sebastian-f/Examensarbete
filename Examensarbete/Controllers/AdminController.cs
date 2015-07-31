using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using Examensarbete.Models;

namespace Examensarbete.Controllers
{
    public class AdminController : Controller
    {
        private ICategoryRepository categoryRepository;        
        public AdminController(ICategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;        
        }

        public ActionResult Categories()
        {
            return View(categoryRepository.Categories);
        }

        public ActionResult Category(int id)
        {
            Category category = categoryRepository.Get(id);
            return View(category);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            Category category = new Category();
            return View(category);
        }

        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            Category c = categoryRepository.Add(category);
            return RedirectToAction("Category", new { id=c.Id });
        }

        [HttpGet]
        public ActionResult AddPriceToCategory(int id,string date=null)
        {
            DateTime dtDate;
            if (date == null) date = DateTime.Now.ToString("yyyy-MM-dd");
            bool dateValid = DateTime.TryParse(date, out dtDate);
            dtDate = dateValid ? dtDate : DateTime.Now;
            Category category = categoryRepository.Get(id);
            AddPriceViewModel addPriceModel;
            if(category.PricePerDay.ToList().Find(m=>m.CheckinDate==dtDate)==null)
                addPriceModel = new AddPriceViewModel() { CategoryId=id,FirstDay=dtDate,LastDay=dtDate,Price=0};
            else
                addPriceModel = new AddPriceViewModel() { CategoryId = id, FirstDay = dtDate, LastDay = dtDate, Price = category.PricePerDay.ToList().Find(m=>m.CheckinDate==dtDate).Price };
            return View(addPriceModel);
        }
        [HttpPost]
        public ActionResult AddPriceToCategory(AddPriceViewModel prices)
        {
            if (prices.CategoryId != 0)
            {
                DateTime day = prices.FirstDay;
                while (day <= prices.LastDay)
                {
                    categoryRepository.AddOrUpdateDaypriceForCategory(prices.CategoryId, prices.Price, day);
                    day = day.AddDays(1);
                }
                return RedirectToAction("Category", new { id = prices.CategoryId });
            }

            return RedirectToAction("Categories");
        }
    }
}