using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.ViewModels
{
    public class HandleImagesViewModel
    {
        public Service.DTO.ImageModel NewImage { get; set; }

        public IList<Service.DTO.ImageModel> Images { get; set; }
        public int CategoryId { get; set; }
    }
}