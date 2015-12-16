using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Domain.Concrete
{
    //TODO: Skriv komentarer här om hur "save(add)" booking fungerar
    public class BookingRepository : IBookingRepository
    {
        private EFDbContext context = new EFDbContext();

        //TODO: Mabye move some code to service
        //Check if there are enought available rooms in a category
        //First check if category has a price on the choosen dates
        public bool CheckAvailableRooms(int categoryId, int numberOfRooms, DateTime checkinDate, DateTime checkOutDate)
        {
            //Check if all days between checkIn and ckeckOut have a price
            Category category = context.Categories.Where(c => c.Id == categoryId).Include(p=>p.PricePerDay).First();
            IList<DatePrice> pricesBetweenDays = category.PricePerDay.Where(pr => pr.CheckinDate >= checkinDate && pr.CheckinDate < checkOutDate).ToList();
            if (pricesBetweenDays.Count() != (checkOutDate - checkinDate).TotalDays) return false;
            
            IList<Room> allRoomsInCategory = (from rm in context.Rooms.Include(bk => bk.Bookings)
                                                  where rm.TheCategory.Id == categoryId
                                                  select rm).ToList();
            int numberOfAvailableRooms = 0;
            foreach(Room room in allRoomsInCategory)
            {
                //Count all bookings in a room, that is on "some of the same days" inside checkin and checkout
                //If there are zero then the room is available
                if (room.Bookings.Where(b => !(b.CheckInDate >= checkOutDate || checkinDate >= b.CheckOutDate)).Count() == 0)
                    numberOfAvailableRooms++;
            }

            //If there are more available rooms then numberOfRooms
            if (numberOfAvailableRooms >= numberOfRooms)
                return true;
            else
                return false;


        }
        
        //Returns the new id if success, and "0" if not
        //TODO: Refactor?
        public int SaveBooking(Booking booking)
        {
            Booking bookingToSave = new Booking() { Rooms=new List<Room>()};
            ICollection<Category> allCategories = context.Categories.ToList();
            int numberOfRoomsInCategory;
            int[] roomIds;
            foreach(Category c in allCategories)
            {
                numberOfRoomsInCategory = booking.Rooms.Where(r => r.TheCategory.Id == c.Id).Count();
                if(numberOfRoomsInCategory>0)
                {
                    roomIds = FindAvailableRooms(c.Id, booking.CheckInDate, booking.CheckOutDate).ToArray();
                    if(roomIds.Count()>=numberOfRoomsInCategory)
                    {
                        for (int j = 0; j < numberOfRoomsInCategory; j++)
                        {
                            bookingToSave.Rooms.Add(context.Rooms.Find(roomIds[j]));
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            bookingToSave.CheckInDate = booking.CheckInDate;
            bookingToSave.CheckOutDate = booking.CheckOutDate;
            bookingToSave.UserId = booking.UserId;


            context.Bookings.Add(bookingToSave);
            context.SaveChanges();

            return bookingToSave.Id;
        }

        //Finds available rooms with categoryId and return room ids
        private ICollection<int> FindAvailableRooms(int categoryId,DateTime inDate,DateTime outDate)
        {
            //TODO:Test if null, ex rooms = null
            bool bookingExists;
            ICollection<Room> rooms = context.Rooms.Where(r => r.TheCategory.Id == categoryId).Include(b=>b.Bookings).ToList();
            ICollection<int> availableRoomIds = new List<int>();
            foreach(Room room in rooms)
            {
                bookingExists = room.Bookings.Where(b => !(b.CheckInDate >= outDate
                                           || inDate >= b.CheckOutDate)
                                           ).Any();
                if (bookingExists == false) availableRoomIds.Add(room.Id);
            }

            return availableRoomIds;
        }


        public IEnumerable<Booking> GetAllBookingsForUser(string userId)
        {
            return context.Bookings.Where(b => b.UserId == userId);
        }



        public Booking GetBooking(int bookingId)
        {
            Booking booking = context.Bookings.Include(b=>b.Rooms.Select(r=>r.TheCategory)).Where(b => b.Id == bookingId).FirstOrDefault();
            
            return booking;
        }
    }
}
