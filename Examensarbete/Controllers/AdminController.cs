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
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        private ICategoryRepository categoryRepository;        
        public AdminController(ICategoryRepository categoryRepository) 
        {
            this.categoryRepository = categoryRepository;        
        }
        public ActionResult Index()
        {
            return View();
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
        public ActionResult AddCategory(Category category, IEnumerable<HttpPostedFileBase> images)
        {
            Domain.Entities.Image imageToSave = new Domain.Entities.Image();
            category.Images = new List<Domain.Entities.Image>();
            if (images != null)
            {
                foreach (HttpPostedFileBase image in images)
                {
                    imageToSave = new Domain.Entities.Image();
                    imageToSave.ImageData = new byte[image.ContentLength];
                    imageToSave.ImageMimeType = image.ContentType;
                    image.InputStream.Read(imageToSave.ImageData, 0, image.ContentLength);
                    category.Images.Add(imageToSave);
                }
            }
            
            Category c = categoryRepository.Add(category);

            return RedirectToAction("Category", new { id = c.Id });
        }

        public FileContentResult GetImage(int imageId,int categoryId) 
        {
            Category category = categoryRepository.Get(categoryId);
            if (category != null) { 
                if(category.Images!= null)
                    return File(category.Images.Where(i=>i.Id==imageId).First().ImageData, category.Images.Where(im=>im.Id==imageId).First().ImageMimeType); 
            }
            return null;
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
        public ActionResult Room()
        {
            IEnumerable<Room> rooms = categoryRepository.GetAllRooms();
            IEnumerable<SelectListItem> categories = categoryRepository.Categories.Select(c=>
                                                       new SelectListItem() {
                                                           Text = c.Name, 
                                                           Value = c.Id.ToString() 
                                                       }
                                                    ).ToList();
            ViewModels.RoomViewModel viewModel = new ViewModels.RoomViewModel() { Rooms=rooms,Categories=categories};
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UpdateRoom(Room roomModel, int categoryId)
        {
            roomModel.TheCategory = new Category(){Id=categoryId};
            categoryRepository.UpdateRoom(roomModel);
            return RedirectToAction("Room");
        }

        [HttpGet]
        public ActionResult AddRoom()
        {
            AddRoomViewModel addRoomModel = new AddRoomViewModel();
            List<SelectListItem> listItems = new List<SelectListItem>();
            IEnumerable<Category> categories = categoryRepository.Categories.ToList();
            foreach(Category category in categories)
            {
                listItems.Add(new SelectListItem() { Text = category.Name, Value = category.Id.ToString() });
            }
            addRoomModel.Categories = listItems;
            return View(addRoomModel);
        }
        [HttpPost]
        public ActionResult AddRoom(AddRoomViewModel addRoomModel,int categoryId)
        {
            Room room = new Room() { RoomNumber = addRoomModel.RoomNumber };
            categoryRepository.AddRoom(room, categoryId);
            return RedirectToAction("Room");
        }
    }
}

