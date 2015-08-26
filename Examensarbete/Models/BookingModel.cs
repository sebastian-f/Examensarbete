using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public virtual ICollection<RoomCategory> RoomCategories { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Double Price { get; set; }
    }
}