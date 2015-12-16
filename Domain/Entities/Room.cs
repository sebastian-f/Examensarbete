using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        //TODO: Ta bort Diplay
        [Display(Name="Rumsnummer")]
        [Required]
        public string  RoomNumber { get; set; }
        [Required]
        public Category TheCategory { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
