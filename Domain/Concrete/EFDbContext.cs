using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Domain.Entities;

namespace Domain.Concrete
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base(ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
