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
    //TODO:Refactoring, too much code in many actionmethods. Create a bookingService
    [Authorize(Roles="Guest")]
    public class BookingController : Controller
    {
        ICategoryRepository categoryRepository;
        IBookingRepository bookingRepository;
        IBookingService bookingService;
        public BookingController(ICategoryRepository categoryRepository,IBookingRepository bookingRepository,IBookingService bookingService)
        {
            this.categoryRepository = categoryRepository;
            this.bookingRepository = bookingRepository;
            this.bookingService = bookingService;
        }
        //TODO: Finish
        //Show all bookings for logged in user
        public ActionResult MyBookings()
        {
            IList<Booking> bookingEntities = bookingRepository.GetAllBookingsForUser(User.Identity.GetUserId()).ToList();
            IList<BookingModel> bookings = bookingEntities.Select(b => new BookingModel
                                             {
                                                 CheckInDate =  b.CheckInDate,
                                                 CheckOutDate = b.CheckOutDate,
                                                 Id = b.Id
                                              }).ToList();
            return View(bookings);
        }
        //Show specific booking for logged in user
        public ActionResult MyBooking(int id)
        {
            Booking bookingEntity = bookingRepository.GetBooking(id);
            if (bookingEntity == null) return RedirectToAction("MyBookings");
            if (bookingEntity.UserId != User.Identity.GetUserId()) return RedirectToAction("MyBookings");
            BookingModel booking = new BookingModel() {CheckInDate=bookingEntity.CheckInDate,CheckOutDate=bookingEntity.CheckOutDate,Id=bookingEntity.Id };
            booking.RoomCategories = new List<RoomCategory>();
            foreach(Room room in bookingEntity.Rooms)
            {
                RoomCategory rc = booking.RoomCategories.Where(r => r.CategoryId == room.TheCategory.Id).FirstOrDefault();
                if (rc == null) booking.RoomCategories.Add(new RoomCategory() { CategoryId=room.TheCategory.Id, Description=room.TheCategory.Description, Name=room.TheCategory.Name, NumberOfRooms=0});
                booking.RoomCategories.Where(cat => cat.CategoryId == room.TheCategory.Id).First().NumberOfRooms++;
            }

            return View(booking);
        }

        // GET: Booking
        [AllowAnonymous]
        public ActionResult Index()
        {
            MakeBooking booking = GetMakeBookingSession();
            if (booking.CheckInDate<DateTime.Now.Date) booking.CheckInDate = DateTime.Now;
            if (booking.CheckOutDate<DateTime.Now.Date) booking.CheckOutDate = booking.CheckInDate.AddDays(1);
            return View(booking);
        }
        //Post checkin and checkout date
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(MakeBooking booking)
        {
            MakeBooking realSession = GetMakeBookingSession();
            if((realSession.CheckInDate.CompareTo(booking.CheckInDate) == 0) && realSession.CheckOutDate.CompareTo(booking.CheckOutDate) == 0)
                return RedirectToAction("Rooms");

            MakeBooking newBooking = new MakeBooking(); 
            newBooking.CheckInDate = booking.CheckInDate;
            newBooking.CheckOutDate = booking.CheckOutDate;
            SaveMakeBookingSession(newBooking);
            return RedirectToAction("Rooms");
        }

        //Choose number of rooms in each category
        [AllowAnonymous]
        public ActionResult Rooms()
        {
            //TODO: Visa bara rum som har priser
            MakeBooking booking = GetMakeBookingSession();
            if(booking.CheckInDate<DateTime.Now.Date || booking.CheckOutDate<=booking.CheckInDate)
            {
                return RedirectToAction("Index");
            }
            else
            {
                double price;
                List<RoomCategory> newCategories = new List<RoomCategory>();
                //Get price, and only display categories with a price for each day
                foreach (RoomCategory category in booking.RoomCategories)
                {
                    if(categoryRepository.HasPriceForAllDays(category.CategoryId,booking.CheckInDate,booking.CheckOutDate)){
                        price = categoryRepository.GetPriceForDates(category.CategoryId, booking.CheckInDate, booking.CheckOutDate);
                        booking.RoomCategories.Where(c => c.CategoryId == category.CategoryId).First().PriceForChoosenDates = price;
                        if (category.Images == null) 
                            booking.RoomCategories.Where(cat => cat.CategoryId == category.CategoryId).First().Images = new List<Models.Image>();
                        newCategories.Add(booking.RoomCategories.Where(c => c.CategoryId == category.CategoryId).First());
                    }
                }
                booking.RoomCategories = newCategories;
            }

            if (TempData["isAvailable"] != null)
            {
                if (TempData["isAvailable"].ToString() == "false") ViewBag.ErrorMessage = "Det finns inte tillräckligt med lediga rum under perioden du valt. Prova ett annat datum eller typ av rum.";
            }

            return View(booking);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Rooms(MakeBooking makeBooking)
        {
            MakeBooking booking = GetMakeBookingSession();
            //if(booking.RoomCategories.Count != makeBooking.RoomCategories.Count) return RedirectToAction("Rooms");
            foreach(RoomCategory roomCategory in makeBooking.RoomCategories)
            {
                //int nrRooms = makeBooking.RoomCategories.Where(r => r.CategoryId == roomCategory.CategoryId).First().NumberOfRooms;
                booking.RoomCategories.Where(r => r.CategoryId == roomCategory.CategoryId).First().NumberOfRooms = roomCategory.NumberOfRooms;
            }
            SaveMakeBookingSession(booking);

            #region checkIfAvailable
            bool available;
            foreach (RoomCategory roomCategory in booking.RoomCategories)
            {
                if (roomCategory.NumberOfRooms > 0)
                {
                    available = bookingRepository.CheckAvailableRooms(roomCategory.CategoryId, roomCategory.NumberOfRooms, booking.CheckInDate, booking.CheckOutDate);

                    if (available == false)
                    {
                        TempData["isAvailable"] = "false";
                        return RedirectToAction("Rooms");
                    }

                }
            }
            #endregion

            return RedirectToAction("Confirm");
        }
         [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            MakeBooking bookingSession = GetMakeBookingSession();
            bool available;
            int numberOfRooms = bookingSession.RoomCategories.Sum(r => r.NumberOfRooms);
            if (numberOfRooms < 1) return RedirectToAction("Rooms"); 
            foreach (RoomCategory roomCategory in bookingSession.RoomCategories)
            {
                if (roomCategory.NumberOfRooms > 0)
                {
                    available = bookingRepository.CheckAvailableRooms(roomCategory.CategoryId, roomCategory.NumberOfRooms, bookingSession.CheckInDate, bookingSession.CheckOutDate);
                    //If there are no available rooms in category, redirect to rooms
                    //TODO: Error message: "change date or roomType(category)"
                    if (available == false)
                    {
                        TempData["isAvailable"] = false;
                        return RedirectToAction("Rooms"); 
                    }

                }
            }
             //TODO: The price should be calculated in a service-method
            bookingSession.Price = bookingSession.RoomCategories.Sum(c=>c.NumberOfRooms*c.PriceForChoosenDates);
            return View(bookingSession);
        }
        [HttpPost]
        public ActionResult Confirm(MakeBooking makeBooking=null)
        {
            makeBooking = GetMakeBookingSession();
            Booking booking = new Booking() {CheckInDate=makeBooking.CheckInDate,CheckOutDate=makeBooking.CheckOutDate};
            booking.Rooms = new List<Room>();
            foreach(RoomCategory room in makeBooking.RoomCategories)
            {
                for (int i = 0; i < room.NumberOfRooms; i++)
                {
                    booking.Rooms.Add(new Room() { TheCategory = new Category() { Id = room.CategoryId } });
                }
            }
            //If there are no categories/rooms, send back to Rooms
            if (booking.Rooms.Count() == 0)
                return RedirectToAction("Rooms");

            booking.UserId = User.Identity.GetUserId();
            int bookingId = bookingRepository.SaveBooking(booking);
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
            //TODO: Try, catch?
            MakeBooking makeBookingModel = (MakeBooking)Session["booking"];
            if(makeBookingModel == null)
            {
                makeBookingModel = new MakeBooking();
                makeBookingModel.RoomCategories = new List<Models.RoomCategory>();
            }
            IList<Category> categories = categoryRepository.Categories.ToList();
            if (makeBookingModel.RoomCategories.Count() == 0)
            {
                foreach (Category category in categories)
                {
                    Models.RoomCategory roomCategory = new Models.RoomCategory() { CategoryId = category.Id, Name = category.Name, Description = category.Description, NumberOfRooms = 0 };
                    roomCategory.Images = new List<Models.Image>();
                    foreach(Domain.Entities.Image img in category.Images)
                    {
                        roomCategory.Images.Add(new Models.Image() { Id = img.Id, ImageData = img.ImageData, ImageMimeType = img.ImageMimeType, Info = img.Info });
                    }
                    
                    makeBookingModel.RoomCategories.Add(roomCategory);
                }
            }

            return makeBookingModel;
        }
        [AllowAnonymous]
        public FileContentResult GetImage(int categoryId,int imageId)
        {
            Category category = categoryRepository.Get(categoryId);
            if (category != null)
            {
                if (category.Images != null)
                    return File(category.Images.Where(i => i.Id == imageId).First().ImageData, category.Images.Where(im => im.Id == imageId).First().ImageMimeType);
            }
            return null;
        }
        
        private void SaveMakeBookingSession(MakeBooking booking)
        {
            Session["booking"] = booking;
        }

        //TODO: REMOVE
        //TESTMETHOD
        [AllowAnonymous]
        public string TestHasPrice()
        { 
            DateTime checkin = DateTime.Parse("2015-10-12");
            DateTime checkout = DateTime.Parse("2015-10-12");
            return categoryRepository.HasPriceForAllDays(1, checkin, checkout).ToString();

        }
    }
}