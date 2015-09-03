using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examensarbete.ViewModels
{
    public class RoomViewModel
    {
        public IEnumerable<Domain.Entities.Room> Rooms { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}