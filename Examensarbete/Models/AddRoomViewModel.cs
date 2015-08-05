using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Examensarbete.Models
{
    public class AddRoomViewModel
    {
        public IEnumerable<SelectListItem> Categories { get; set; }
        public string RoomNumber { get; set; }
    }
}