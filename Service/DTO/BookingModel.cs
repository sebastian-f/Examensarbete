using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class BookingModel
    {
        public int Id { get; set; }
        public virtual ICollection<RoomModel> Rooms { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string UserId { get; set; }
    }
}
