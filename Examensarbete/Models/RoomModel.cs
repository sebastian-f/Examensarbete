using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Du måste välja ett namn på rummet")]
        public string RoomNumber { get; set; }
    }
}