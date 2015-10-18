using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.ViewModels
{
    public class HandleImagesViewModel
    {
        public Models.Image NewImage { get; set; }

        public IList<Models.Image> Images { get; set; }
        public int CategoryId { get; set; }
    }
}