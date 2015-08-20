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
    public class BookingRepository : IBookingRepository
    {
        private EFDbContext context = new EFDbContext();

        public bool CheckAvailableRooms(int categoryId, int numberOfRooms, DateTime checkinDate, DateTime checkOutDate)
        {
           
            IQueryable<Booking> bookings = from bks in context.Bookings.Include(b => b.Rooms.Select(c => c.TheCategory))
                                           where !(bks.CheckInDate >= checkOutDate
                                           || checkinDate >= bks.CheckOutDate)
                                           select bks;
            
            int numberOfRoomsInCategory = (from rms in context.Rooms
                                          where rms.TheCategory.Id==categoryId
                                          select rms).Count();
            int roomsOfSameCategory = 0;
            foreach (Booking booking in bookings)
            {
                roomsOfSameCategory += (from b in booking.Rooms
                                        where b.TheCategory.Id == categoryId
                                        select b).Count();
                                      

            }
            if ((numberOfRoomsInCategory - roomsOfSameCategory) >= numberOfRooms)
                return true;
            else
                return false;
                        
        }
        //TODO:Refactoring?

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
    }
}
