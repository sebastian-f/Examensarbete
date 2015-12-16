using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Examensarbete.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Display(Name="Namn på kategorin")]
        [Required(ErrorMessage="Du måste välja ett namn på kategorin")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DatePriceModel> PricePerDay { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}