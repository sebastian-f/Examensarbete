using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IBookingService
    {
        IEnumerable<BookingModel> GetBookingsForUser(string userId);
        BookingModel GetBooking(int id);
        bool CheckAvailableRooms(int categoryId,int numberOfRooms,DateTime checkInDate,DateTime checkOutDate);
        int SaveBooking(BookingModel bookingModel);
    }
}
