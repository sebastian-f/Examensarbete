using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Service.DTO
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Display(Name = "Namn på kategorin")]
        [Required(ErrorMessage = "Du måste välja ett namn på kategorin")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DatePriceModel> PricePerDay { get; set; }
        public virtual ICollection<ImageModel> Images { get; set; }
    }
}
