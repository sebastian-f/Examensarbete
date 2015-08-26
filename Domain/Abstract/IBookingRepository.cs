using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface IBookingRepository
    {
        bool CheckAvailableRooms(int categoryId,int numberOfRooms,DateTime checkinDate,DateTime checkOutDate);
        int SaveBooking(Booking booking);
        IEnumerable<Booking> GetAllBookingsForUser(string userId);
        Booking GetBooking(int bookingId);
    }
}
