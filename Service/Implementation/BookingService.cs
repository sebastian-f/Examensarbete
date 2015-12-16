using Domain.Abstract;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.DTO;
using Domain.Entities;
using AutoMapper;

namespace Service.Implementation
{
    public class BookingService : IBookingService
    {
        private IBookingRepository bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            this.bookingRepository = bookingRepository;
        }

        public IEnumerable<BookingModel> GetBookingsForUser(string userId)
        {
            IEnumerable<Booking> bookings = bookingRepository.GetAllBookingsForUser(userId);
            IEnumerable<BookingModel> bookingModels = Mapper.Map<IEnumerable<Booking>, IEnumerable<BookingModel>>(bookings);
            return bookingModels;
        }


        public BookingModel GetBooking(int id)
        {
            Booking booking = bookingRepository.GetBooking(id);
            BookingModel bookingModel = Mapper.Map<Booking, BookingModel>(booking);
            return bookingModel;
        }



        public bool CheckAvailableRooms(int categoryId, int numberOfRooms, DateTime checkInDate, DateTime checkOutDate)
        {
            return bookingRepository.CheckAvailableRooms(categoryId, numberOfRooms, checkInDate, checkOutDate);
        }


        public int SaveBooking(BookingModel bookingModel)
        {
            Booking bookingEntity = Mapper.Map<BookingModel, Booking>(bookingModel);
            int bookingId = bookingRepository.SaveBooking(bookingEntity);
            return bookingId;
        }
    }
}
