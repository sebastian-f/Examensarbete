﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DatePrice> PricePerDay { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

    }
}
