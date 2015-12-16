using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    //TODO: Rename to ViewModel and move to ViewModels map
    //TODO: Motsvarande metod för BookingModelToViewModel, fast åt andra hållet: BookingModel(View) till bookingModel(Service)
    public class BookingModel
    {
        public int Id { get; set; }
        public virtual ICollection<RoomCategory> RoomCategories { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Double Price { get; set; }

        public BookingModel()
        {

        }
        public BookingModel(Service.DTO.BookingModel bookingModel)
        {
            this.CheckInDate = bookingModel.CheckInDate;
            this.CheckOutDate = bookingModel.CheckOutDate;
            this.Id = bookingModel.Id;
            this.Price = 0;
            this.RoomCategories = new List<RoomCategory>();
            foreach (Service.DTO.RoomModel room in bookingModel.Rooms)
            {
                RoomCategory rc = this.RoomCategories.Where(r => r.CategoryId == room.TheCategory.Id).FirstOrDefault();
                if (rc == null) this.RoomCategories.Add(new RoomCategory() { CategoryId = room.TheCategory.Id, Description = room.TheCategory.Description, Name = room.TheCategory.Name, NumberOfRooms = 0 });
                this.RoomCategories.Where(cat => cat.CategoryId == room.TheCategory.Id).First().NumberOfRooms++;
            }
        }
        BookingModel BookingModelToViewModel(Service.DTO.BookingModel bookingModel)
        {
            BookingModel bookingViewModel = new BookingModel() { CheckInDate = bookingModel.CheckInDate, CheckOutDate = bookingModel.CheckOutDate, Id = bookingModel.Id, Price = 0 };
            foreach (Service.DTO.RoomModel room in bookingModel.Rooms)
            {
                RoomCategory rc = bookingViewModel.RoomCategories.Where(r => r.CategoryId == room.TheCategory.Id).FirstOrDefault();
                if (rc == null) bookingViewModel.RoomCategories.Add(new RoomCategory() { CategoryId = room.TheCategory.Id, Description = room.TheCategory.Description, Name = room.TheCategory.Name, NumberOfRooms = 0 });
                bookingViewModel.RoomCategories.Where(cat => cat.CategoryId == room.TheCategory.Id).First().NumberOfRooms++;
            }
            return bookingViewModel;
        }
    }
}