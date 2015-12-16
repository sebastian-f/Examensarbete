using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class MakeBooking
    {
        public int Id { get; set; }
        //TODO:Date without time, attribute
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CheckInDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CheckOutDate { get; set; }
        public double Price { get; set; }
        public IList<RoomCategory> RoomCategories { get; set; }

        public MakeBooking()
        {
            RoomCategories = new List<RoomCategory>();
        }
    }

    
    public class RoomCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double PriceForChoosenDates { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int NumberOfRooms { get; set; }
        public IList<Image> Images { get; set; }

        //public RoomCategory()
        //{
        //    NumberOfRooms = 0;
        //    Images = new List<Image>();
        //}
    }

}

