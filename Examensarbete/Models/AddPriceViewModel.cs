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
        public double Price { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
    }
}