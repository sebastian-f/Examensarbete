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
using AutoMapper;
using Service.Interface;

namespace Examensarbete.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        private ICategoryService categoryService;
        public AdminController(ICategoryService categoryService) 
        {
            this.categoryService = categoryService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Categories()
        {
            IEnumerable<Service.DTO.CategoryModel> categoryModels = categoryService.GetAllCategories();
            
            
            return View(categoryModels);
        }

        public ActionResult Category(int id)
        {
            Service.DTO.CategoryModel categoryModel = categoryService.GetById(id);
            return View(categoryModel);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            Service.DTO.CategoryModel categoryModel = new Service.DTO.CategoryModel();
            return View(categoryModel);
        }

        [HttpPost]
        public ActionResult AddCategory(Service.DTO.CategoryModel category, IEnumerable<HttpPostedFileBase> images)
        {
            if(!ModelState.IsValid)
            {
                return View(category);
            }

            //Service.DTO.ImageModel imageToSave = new Service.DTO.ImageModel();
            //category.Images = new List<Service.DTO.ImageModel>();
            //if (images != null)
            //{
            //    foreach (HttpPostedFileBase image in images)
            //    {
                    
            //        if (image != null)
            //        {
            //            //TODO: Felmeddelande om filen inte är en bild
            //            if (IsImage(image) == false) return RedirectToAction("AddCategory");

            //            imageToSave = new Service.DTO.ImageModel();
            //            imageToSave.ImageData = new byte[image.ContentLength];
            //            imageToSave.ImageMimeType = image.ContentType;
            //            image.InputStream.Read(imageToSave.ImageData, 0, image.ContentLength);
            //            category.Images.Add(imageToSave);

            //        }
            //    }
            //}

            Service.DTO.CategoryModel cat = categoryService.SaveCategory(category);
            //TODO: Maybe check if 'cat' is null, meaning it wasn't saved

            return RedirectToAction("Category", new { id = cat.Id });
        }
        public ActionResult UpdateCategory(Service.DTO.CategoryModel categoryModel )
        {
            if (!ModelState.IsValid) return View(categoryModel);

            categoryService.UpdateCategoryNameAndDesc(categoryModel);
            return RedirectToAction("Category", new { id = categoryModel.Id });
        }

        public FileContentResult GetImage(int imageId,int categoryId) 
        {
            Service.DTO.ImageModel imageModel = categoryService.GetImage(categoryId, imageId);
            return File(imageModel.ImageData, imageModel.ImageMimeType); 
        }

        [HttpGet]
        public ActionResult AddPriceToCategory(int id,string date)
        {
            DateTime dtDate;
            bool dateValid = DateTime.TryParse(date, out dtDate);
            dtDate = dateValid ? dtDate : DateTime.Now;

            Service.DTO.CategoryModel categoryModel = categoryService.GetById(id);
            AddPriceViewModel addPriceModel = new AddPriceViewModel() { CategoryId = id, FirstDay = dtDate, LastDay = dtDate,Price=0 };
            if (categoryModel.PricePerDay.Any(m => m.CheckinDate.Date == dtDate.Date))
                addPriceModel.Price = categoryModel.PricePerDay.First(m => m.CheckinDate.Date == dtDate.Date).Price;
            
            return View(addPriceModel);
        }
        [HttpPost]
        public ActionResult AddPriceToCategory(AddPriceViewModel prices)
        {
            //TODO: Sätta en maxgräns för hur många dagar man kan spara pris för. ?
            if (!ModelState.IsValid) return View(prices);

            categoryService.AddOrUpdateCategoryPrice(prices.CategoryId, prices.Price, prices.FirstDay, prices.LastDay);
            return RedirectToAction("Category", new { id = prices.CategoryId });

        }
        public ActionResult Room()
        {
            IEnumerable<Service.DTO.RoomModel> rooms = categoryService.GetAllRooms();
            IEnumerable<SelectListItem> categories = categoryService.GetAllCategories().Select(c=>
                                                       new SelectListItem() {
                                                           Text = c.Name, 
                                                           Value = c.Id.ToString() 
                                                       }
                                                    ).ToList(); 
            ViewModels.RoomViewModel viewModel = new ViewModels.RoomViewModel() { Rooms=rooms,Categories=categories};
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UpdateRoom(Service.DTO.RoomModel roomToUpdate, int categoryId)
        {
            if (String.IsNullOrEmpty(roomToUpdate.RoomNumber)) return RedirectToAction("Room");
            roomToUpdate.TheCategory = new Service.DTO.CategoryModel() { Id = categoryId };
            categoryService.UpdateRoom(roomToUpdate);
            return RedirectToAction("Room");
        }

        [HttpGet]
        public ActionResult AddRoom()
        {
            AddRoomViewModel addRoomModel = new AddRoomViewModel();
            List<SelectListItem> listItems = new List<SelectListItem>();
            IEnumerable<Service.DTO.CategoryModel> categories = categoryService.GetAllCategories().ToList();
            foreach(Service.DTO.CategoryModel category in categories)
            {
                listItems.Add(new SelectListItem() { Text = category.Name, Value = category.Id.ToString() });
            }
            addRoomModel.Categories = listItems;
            return View(addRoomModel);
        }
        [HttpPost]
        public ActionResult AddRoom(AddRoomViewModel addRoomModel,int categoryId)
        {
            if (!ModelState.IsValid) return RedirectToAction("AddRoom");
            Service.DTO.RoomModel room = new Service.DTO.RoomModel() 
                                                    { RoomNumber = addRoomModel.RoomNumber, 
                                                      TheCategory = new Service.DTO.CategoryModel() { Id=categoryId} 
                                                    };
            categoryService.AddRoom(room);
            return RedirectToAction("Room");
        }

        //TODO:Rename action method, to categoryImages??
        //Handle images in a category
        //No point of loading ImageMimeType and ImageData
        public ActionResult Images(int id)
        {
            HandleImagesViewModel handleImages = new HandleImagesViewModel();
            handleImages.Images = (from img in categoryService.GetById(id).Images
                                  select new Service.DTO.ImageModel() { Id = img.Id, Info = img.Info }).ToList();
            handleImages.CategoryId = id;
            ViewBag.catId = id;
            return View(handleImages);
        }

        [HttpPost]
        public ActionResult DeleteImage(int imageId, int categoryId)
        {
            categoryService.DeleteImage(imageId, categoryId);
            return RedirectToAction("Images", new { id=categoryId});
        }

        [HttpPost]
        public ActionResult UpdateImage(Service.DTO.ImageModel imageModel,int categoryId)
        {
            categoryService.UpdateImageInfo(imageModel, categoryId);
            return RedirectToAction("Images", new { id = categoryId });
        }

        [HttpPost]
        public ActionResult AddImage(HttpPostedFileBase imageFile,HandleImagesViewModel handleImg,int categoryId)
        {
            //Ingen bild vald, felmeddelande
            if (imageFile == null) return RedirectToAction("Images", new { id = categoryId });

            //TODO: Felmeddelande om filen inte är en bild
            if (IsImage(imageFile) == false) return RedirectToAction("Images", new { id = categoryId });

            Service.DTO.ImageModel imageModel = new Service.DTO.ImageModel();
            imageModel.Info = handleImg.NewImage.Info;
            imageModel.ImageData = new byte[imageFile.ContentLength];
            imageModel.ImageMimeType = imageFile.ContentType;
            imageFile.InputStream.Read(imageModel.ImageData, 0, imageFile.ContentLength);
            categoryService.AddImageToCategory(imageModel, categoryId);
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

