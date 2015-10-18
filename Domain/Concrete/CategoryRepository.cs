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
                IEnumerable<Category> categories = context.Categories.Include(p =>  p.PricePerDay).Include(i=>i.Images);
                return context.Categories.Include(i=>i.Images);
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
            Category category = context.Categories.Include(i=>i.Images).FirstOrDefault(c=>c.Id==id);
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


        public Image GetImage(int imageId)
        {
            //TODO:
            throw new NotImplementedException();
        }


        public void UpdateCategoryNameAndInfo(int id, string name, string info)
        {
            Category category = context.Categories.Where(c => c.Id == id).First();
            category.Name = name;
            category.Description = info;
            context.SaveChanges();
        }


        public void DeleteImage(int imageId,int categoryId)
        {
            Category category = context.Categories.Where(c => c.Id == categoryId).First();
            Image image = category.Images.Where(im => im.Id == imageId).First();
            category.Images.Remove(image);
            context.Images.Remove(image);
            context.SaveChanges();
        }

        public void UpdateImage(Image image,int categoryId)
        {
            Category category = context.Categories.Where(c => c.Id == categoryId).First();
            category.Images.Where(im => im.Id == image.Id).First().Info = image.Info;
            context.SaveChanges();
        }


        public void AddImageToCategory(Image image, int categoryId)
        {
            Category category = context.Categories.Where(c => c.Id == categoryId).First();
            category.Images.Add(image);
            context.SaveChanges();
        }


        public bool HasPriceForAllDays(int categoryId, DateTime checkInDate, DateTime checkOutDate)
        {
            Category category = context.Categories.Find(categoryId);
            context.Entry(category).Collection(p => p.PricePerDay).Load();

            if (category.PricePerDay.Where(d=>d.CheckinDate>=checkInDate && d.CheckinDate<checkOutDate).Count() == (checkOutDate - checkInDate).Days)
                return true;
            else
                return false;
        }
    }
}
