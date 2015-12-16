using Domain.Abstract;
using Domain.Entities;
using Examensarbete.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Service.Interface;

namespace Examensarbete.Controllers
{
    //Make a reservation. 3 steps, choose date(checkin, checkout), select category(rooms), and confirm. 
    //Guest have to be member/logged in to confirm.
    //TODO:Refactoring, too much code in many actionmethods. Move code to bookingService
    [Authorize(Roles="Guest")]
    public class BookingController : Controller
    {
        IBookingService bookingService;
        ICategoryService categoryService;
        public BookingController(IBookingService bookingService, ICategoryService categoryService)
        {
            this.bookingService = bookingService;
            this.categoryService = categoryService;
        }
        //TODO: Finish
        //Show all bookings for logged in user
        public ActionResult MyBookings()
        {
            IList<Service.DTO.BookingModel> bookings = bookingService.GetBookingsForUser(User.Identity.GetUserId()).Select(b => new Service.DTO.BookingModel
                                            {
                                                CheckInDate = b.CheckInDate,
                                                CheckOutDate = b.CheckOutDate,
                                                Id = b.Id
                                            }).ToList(); 
            return View(bookings);
        }
        //Show specific booking for logged in user
        public ActionResult MyBooking(int id)
        {
            Service.DTO.BookingModel bookingModel = bookingService.GetBooking(id);
            if (bookingModel == null) return RedirectToAction("MyBookings");
            if (bookingModel.UserId != User.Identity.GetUserId()) return RedirectToAction("MyBookings");

            BookingModel bookingViewModel = new BookingModel(bookingModel);

            return View(bookingViewModel);
        }

        // GET: Booking
        [AllowAnonymous]
        public ActionResult Index()
        {
            MakeBooking bookingViewModel = GetMakeBookingSession();
            if (bookingViewModel == null)
            {
                bookingViewModel = new MakeBooking() { CheckInDate= DateTime.Now.Date,CheckOutDate=DateTime.Now.AddDays(1).Date};
            }
            return View(bookingViewModel);
        }
        //Post checkin and checkout date
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(MakeBooking booking)
        {
            booking.CheckInDate = booking.CheckInDate.Date;
            booking.CheckOutDate = booking.CheckOutDate.Date;
            if (!ModelState.IsValid || booking.CheckInDate<DateTime.Now.Date || booking.CheckOutDate<=booking.CheckInDate)
            {
                ModelState.AddModelError(string.Empty, "Felaktigt datum. Inchecknig måste vara tidigast idag och utcheckning måste ske efter.");
                return View(booking); 
            }

            ChangeSessionDate(booking);
    
            return RedirectToAction("Rooms");
        }

        //Choose number of rooms in each category
        [AllowAnonymous]
        public ActionResult Rooms()
        {
            MakeBooking booking = GetMakeBookingSession();

            if (TempData["isAvailable"] != null)
            {
                if (TempData["isAvailable"].ToString() == "false")
                {
                    int categoryId = Int32.Parse(TempData["nonAvailableCategoryId"].ToString());
                    Service.DTO.CategoryModel category = categoryService.GetById(categoryId);
                    ViewBag.ErrorMessage = "Det finns inte tillräckligt med lediga rum av typen '" + category.Name + "' under perioden du valt. Prova ett annat datum eller typ av rum.";
                }
            }

            return View(booking);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Rooms(MakeBooking makeBooking)
        {
            //If there are no rooms selected
            int numberOfRooms = makeBooking.RoomCategories.Sum(r => r.NumberOfRooms);
            if (numberOfRooms < 1)
            {
                TempData["NoRoomsSelected"] = "Du har inte valt något rum";
                return RedirectToAction("Rooms"); 
            }

            //If modelstate is false
            if (!ModelState.IsValid) return RedirectToAction("Rooms");

            //Get all categories with 'not enought' rooms available
            //If null: there are enought rooms. If not null: Redirect back to room action with error-message
            RoomCategory nonAvailableRoom = makeBooking.RoomCategories.Where(c =>
                                bookingService.CheckAvailableRooms(c.CategoryId, c.NumberOfRooms, makeBooking.CheckInDate, makeBooking.CheckOutDate)
                                == false
                                ).FirstOrDefault();
            if (nonAvailableRoom != null)
            {
                TempData["isAvailable"] = "false";
                TempData["nonAvailableCategoryId"] = nonAvailableRoom.CategoryId;
                return RedirectToAction("Rooms");
            }

 

            ChangeSessionRooms(makeBooking);

            return RedirectToAction("Confirm");
        }
         [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            MakeBooking bookingSession = GetMakeBookingSession();
            int numberOfRooms = bookingSession.RoomCategories.Sum(r => r.NumberOfRooms);
            if (numberOfRooms < 1) return RedirectToAction("Rooms"); 

             //TODO: The price should be calculated in a service-method
            bookingSession.Price = bookingSession.RoomCategories.Sum(c=>c.NumberOfRooms*c.PriceForChoosenDates);
            return View(bookingSession);
        }
        [HttpPost]
        public ActionResult Confirm(MakeBooking makeBooking=null)
        {
            makeBooking = GetMakeBookingSession();
            Service.DTO.BookingModel booking = new Service.DTO.BookingModel() { CheckInDate = makeBooking.CheckInDate, CheckOutDate = makeBooking.CheckOutDate };
            booking.Rooms = new List<Service.DTO.RoomModel>();
            foreach(RoomCategory room in makeBooking.RoomCategories)
            {
                for (int i = 0; i < room.NumberOfRooms; i++)
                {
                    booking.Rooms.Add(new Service.DTO.RoomModel() { TheCategory = new Service.DTO.CategoryModel() { Id = room.CategoryId } });
                }
            }
            //If there are no categories/rooms, send back to Rooms
            if (booking.Rooms.Count() == 0)
                return RedirectToAction("Rooms");

            booking.UserId = User.Identity.GetUserId();
            int bookingId = bookingService.SaveBooking(booking);
            if (bookingId!=0)
            {
                makeBooking.Id=bookingId;
                SaveMakeBookingSession(makeBooking);
                return RedirectToAction("Completed");
            }
            else
                return View(makeBooking);
        }

        public ActionResult Completed()
        {
            
            MakeBooking booking = GetMakeBookingSession();
            //If someone tries to go to Completed-action without passing Confirm 
            if (booking.Id == 0) return RedirectToAction("Index","Home");

            SaveMakeBookingSession(null);
            return View(booking);
        }



        private MakeBooking GetMakeBookingSession()
        {
            //Check if there is no new updates made after the session was crated. For example, if admin added a new category
            MakeBooking makeBookingModel = (MakeBooking)Session["booking"];
            return makeBookingModel;
        }
        [AllowAnonymous]
        public FileContentResult GetImage(int categoryId,int imageId)
        {
            Service.DTO.CategoryModel category = categoryService.GetById(categoryId);
            if (category != null)
            {
                if (category.Images != null)
                    return File(category.Images.Where(i => i.Id == imageId).First().ImageData, category.Images.Where(im => im.Id == imageId).First().ImageMimeType);
            }
            return null;
        }
        
        private void SaveMakeBookingSession(MakeBooking booking)
        {
            //Lägga till ".Date" för att inte jämföra samma datum med olika tider.. ?
            if (booking.CheckInDate >= DateTime.Now.Date || booking.CheckOutDate > booking.CheckInDate)
            {

                Session["booking"] = booking;
            }
        }

        private void ChangeSessionDate(MakeBooking booking)
        {
            
            MakeBooking savedBookingSession = (MakeBooking)Session["booking"];
            MakeBooking newBooking = new MakeBooking() { CheckInDate=booking.CheckInDate,CheckOutDate=booking.CheckOutDate};
            if(savedBookingSession==null)
            {
                #region  changethiscode
                List<RoomCategory> allCategories = GetAllRoomCategories().ToList(); 
                List<RoomCategory> newCategories = new List<RoomCategory>();
                double price=0;
                //Get price, and only display categories with a price for each day
                foreach (RoomCategory category in allCategories)
                {
                    if (categoryService.HasPriceForDays(category.CategoryId, booking.CheckInDate, booking.CheckOutDate))
                    {
                        price = categoryService.GetPriceForDates(category.CategoryId, booking.CheckInDate, booking.CheckOutDate);
                        allCategories.Where(c => c.CategoryId == category.CategoryId).First().PriceForChoosenDates = price;
                        newCategories.Add(allCategories.Where(c => c.CategoryId == category.CategoryId).First());
                    }
                }
                newBooking.RoomCategories = newCategories;
                #endregion
                SaveMakeBookingSession(newBooking);
            }
            else
            {
                //If the dates are not the same, we need to get available rooms for the new dates
                if (booking.CheckInDate.Date != savedBookingSession.CheckInDate.Date && booking.CheckOutDate.Date != savedBookingSession.CheckOutDate.Date)
                {
                    #region  changethiscode
                    List<RoomCategory> allCategories = GetAllRoomCategories().ToList();
                    List<RoomCategory> newCategories = new List<RoomCategory>();
                    double price = 0;
                    //Get price, and only display categories with a price for each day
                    foreach (RoomCategory category in allCategories)
                    {
                        if (categoryService.HasPriceForDays(category.CategoryId, booking.CheckInDate, booking.CheckOutDate))
                        {
                            price = categoryService.GetPriceForDates(category.CategoryId, booking.CheckInDate, booking.CheckOutDate);
                            allCategories.Where(c => c.CategoryId == category.CategoryId).First().PriceForChoosenDates = price;
                            newCategories.Add(allCategories.Where(c => c.CategoryId == category.CategoryId).First());
                        }
                    }
                    newBooking.RoomCategories = newCategories;
                    #endregion
                    SaveMakeBookingSession(newBooking);
                }
                else
                {
                    //If no dates have been changed, we do not need to do anything.
                }
            }
        }

        private void ChangeSessionRooms(MakeBooking booking)
        {
            MakeBooking savedBookingSession = (MakeBooking)Session["booking"];
            //TODO: Check dates and if null

            foreach(RoomCategory room in booking.RoomCategories)
            {
                savedBookingSession.RoomCategories.Where(cat => cat.CategoryId == room.CategoryId).First().NumberOfRooms = room.NumberOfRooms;
            }

            Session["booking"] = savedBookingSession;

        }

        private IEnumerable<RoomCategory> GetAllRoomCategories() 
        {
            List<RoomCategory> roomCategories = new List<RoomCategory>();
            IList<Service.DTO.CategoryModel> categories = categoryService.GetAllCategories().ToList();
                foreach (Service.DTO.CategoryModel category in categories)
                {
                    Models.RoomCategory roomCategory = new Models.RoomCategory() { CategoryId = category.Id, Name = category.Name, Description = category.Description, NumberOfRooms = 0 };
                    roomCategory.Images = new List<Models.Image>();
                    foreach (Service.DTO.ImageModel img in category.Images)
                    {
                        roomCategory.Images.Add(new Models.Image() { Id = img.Id, ImageData = img.ImageData, ImageMimeType = img.ImageMimeType, Info = img.Info });
                    }

                    roomCategories.Add(roomCategory);
                }
                return roomCategories;
        }

      
    }
}