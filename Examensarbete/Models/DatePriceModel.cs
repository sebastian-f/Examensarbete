using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class DatePriceModel
    {
        public int Id { get; set; }
        public DateTime CheckinDate { get; set; }
        public double Price { get; set; }
    }
}