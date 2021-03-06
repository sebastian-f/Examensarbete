﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        //TODO: Delete display name
        [Display(Name="Namn på kategorin")]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DatePrice> PricePerDay { get; set; }
        public virtual ICollection<Image> Images { get; set; }

    }
}
