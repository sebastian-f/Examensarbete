using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DatePrice
    {
        public int Id { get; set; }
        //TODO: [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")] ???
        [Required]
        public DateTime CheckinDate { get; set; }
        [Required]
        public double Price { get; set; }

        //public Category _Category { get; set; }
    }
}
