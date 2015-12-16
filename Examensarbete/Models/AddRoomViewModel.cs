using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Examensarbete.Models
{
    public class AddRoomViewModel
    {
        public IEnumerable<SelectListItem> Categories { get; set; }
        
        [Required(ErrorMessage="Du måste fylla i rumsnummer")]
        public string RoomNumber { get; set; }
    }
}