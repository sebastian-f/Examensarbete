using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class RoomModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Du måste fylla i rumsnummer")]
        public string RoomNumber { get; set; }
        [Required]
        public CategoryModel TheCategory { get; set; }
        //public virtual ICollection<BookingModel> Bookings { get; set; }
    }
}
