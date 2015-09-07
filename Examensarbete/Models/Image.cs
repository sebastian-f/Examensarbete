using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class Image
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
        public string Info { get; set; }
    }
}