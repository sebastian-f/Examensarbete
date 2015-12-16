using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examensarbete.ViewModels
{
    public class RoomViewModel
    {
        public IEnumerable<Service.DTO.RoomModel> Rooms { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public Service.DTO.RoomModel RoomToUpdate { get; set; }
    }
}