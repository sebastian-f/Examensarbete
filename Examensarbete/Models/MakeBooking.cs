using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class MakeBooking
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public double Price { get; set; }
        public IList<RoomCategory> RoomCategories { get; set; }
    }

    
    public class RoomCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PriceForChoosenDates { get; set; }
        public int NumberOfRooms { get; set; }
        public byte[] ImageData { get; set; }        
        public string ImageMimeType { get; set; }
    }

}

