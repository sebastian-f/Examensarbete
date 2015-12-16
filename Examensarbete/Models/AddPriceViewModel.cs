using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class AddPriceViewModel
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Du måste välja ett pris")]
        [Range(0,double.MaxValue,ErrorMessage="Väl ett pris ")]
        public double Price { get; set; }
        [Required(ErrorMessage = "Du måste välja ett startdatum")]
        public DateTime FirstDay { get; set; }
        [Required(ErrorMessage="Du måste välja ett slutdatum")]
        public DateTime LastDay { get; set; }
    }
}