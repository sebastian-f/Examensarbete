using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using Examensarbete.Models;
using Examensarbete.ViewModels;
using System.IO;

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
                    //TODO: Felmeddelande om filen inte är en bild
                    if (IsImage(image) == false) return RedirectToAction("AddCategory");

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
        public ActionResult UpdateCategory(Category category )
        {
            categoryRepository.UpdateCategoryNameAndInfo(category.Id, category.Name, category.Description);
            return RedirectToAction("Category", new{id= category.Id});
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

        //public FileContentResult GetImage(int imageId)
        //{
        //    Domain.Entities.Image imageEntity = categoryRepository.GetImage(imageId);

        //    if (imageEntity != null)
        //        return File(imageEntity.ImageData, imageEntity.ImageMimeType);
        //    else
        //        return null;
        //}

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

        //Handle images in a category
        //No point of loading ImageMimeType and ImageData
        public ActionResult Images(int id)
        {
            HandleImagesViewModel handleImages = new HandleImagesViewModel();
            handleImages.Images = (from img in categoryRepository.Get(id).Images
                                  select new Examensarbete.Models.Image() { Id = img.Id, Info = img.Info }).ToList();
            handleImages.CategoryId = id;
            ViewBag.catId = id;
            return View(handleImages);
        }

        [HttpPost]
        public ActionResult DeleteImage(int imageId, int categoryId)
        {
            categoryRepository.DeleteImage(imageId, categoryId);
            return RedirectToAction("Images", new { id=categoryId});
        }

        [HttpPost]
        public ActionResult UpdateImage(Examensarbete.Models.Image image,int categoryId)
        {
            Domain.Entities.Image imageEntity = new Domain.Entities.Image() { Id=image.Id,Info=image.Info};
            categoryRepository.UpdateImage(imageEntity, categoryId);
            return RedirectToAction("Images", new { id = categoryId });
        }

        [HttpPost]
        public ActionResult AddImage(HttpPostedFileBase imageFile,HandleImagesViewModel handleImg,int categoryId)
        {
            //TODO: Felmeddelande om filen inte är en bild
            if (IsImage(imageFile) == false) return RedirectToAction("Images", new { id = categoryId });

            Domain.Entities.Image imageEntity = new Domain.Entities.Image();
            imageEntity.Info = handleImg.NewImage.Info;
            imageEntity.ImageData = new byte[imageFile.ContentLength];
            imageEntity.ImageMimeType = imageFile.ContentType;
            imageFile.InputStream.Read(imageEntity.ImageData, 0, imageFile.ContentLength);
            categoryRepository.AddImageToCategory(imageEntity, categoryId);
            return RedirectToAction("Images",new {id=categoryId});
        }

        [NonAction]
        private bool IsImage(HttpPostedFileBase imageFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (imageFile.ContentType.ToLower() != "image/jpg" &&
                        imageFile.ContentType.ToLower() != "image/jpeg" &&
                        imageFile.ContentType.ToLower() != "image/pjpeg" &&
                        imageFile.ContentType.ToLower() != "image/gif" &&
                        imageFile.ContentType.ToLower() != "image/x-png" &&
                        imageFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(imageFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(imageFile.FileName).ToLower() != ".png"
                && Path.GetExtension(imageFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(imageFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //If file is an image
            return true;
        }

    }
}

