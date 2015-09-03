using Domain.Abstract;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Domain.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        private EFDbContext context = new EFDbContext();
        public IEnumerable<Category> Categories
        {
            get
            {
                IEnumerable<Category> categories = context.Categories.Include(p => p.PricePerDay);
                return context.Categories;
            }
        }


        public Category Add(Category category)
        {
            if (category.Id == 0)
            {
                context.Categories.Add(category);
                context.SaveChanges();
                return category;
            }
            return null;
        }


        public Category Get(int id)
        {
            Category category = context.Categories.Find(id);
            if(category!=null) context.Entry(category).Collection(p => p.PricePerDay).Load(); 
            return category;
            
        }


        public void AddOrUpdateDaypriceForCategory(int categoryId, double price,DateTime date)
        {
            Category category = context.Categories.Find(categoryId);
            context.Entry(category).Collection(p => p.PricePerDay).Load(); 
            if(category.PricePerDay.ToList().Exists(p=>p.CheckinDate==date))
            {
                category.PricePerDay.ToList().Find(p => p.CheckinDate == date).Price = price;

            }
            else
            {
                DatePrice dayPrice = new DatePrice(){CheckinDate=date,Price=price};
                context.Categories.Find(categoryId).PricePerDay.Add(dayPrice);
            }
            context.SaveChanges();
        }


        public void AddRoom(Room room, int categoryId)
        {
            if(room.Id==0)
            {
                Category category = Get(categoryId);
                room.TheCategory = category;
            }
            context.Rooms.Add(room);
            context.SaveChanges();
        }


        public double GetPriceForDates(int categoryId, DateTime checkInDate, DateTime checkOutDate)
        {
            double price = 0;

            if(checkInDate<checkOutDate)
            {
                Category category = context.Categories.Find(categoryId);
                context.Entry(category).Collection(p => p.PricePerDay).Load();
                price = category.PricePerDay.Where(p => p.CheckinDate >= checkInDate && p.CheckinDate < checkOutDate).Sum(s => s.Price);
            }
            else{
                throw new Exception();
            }
            return price;
        }


        public IEnumerable<Room> GetAllRooms()
        {
            return context.Rooms.Include(c=>c.TheCategory);
        }


        public void UpdateRoom(Room room)
        {
            Room roomEntity = context.Rooms.Where(r => r.Id == room.Id).FirstOrDefault();
            roomEntity.RoomNumber = room.RoomNumber;
            roomEntity.TheCategory = context.Categories.Where(c => c.Id == room.TheCategory.Id).FirstOrDefault();
            context.SaveChanges();
        }
    }
}
