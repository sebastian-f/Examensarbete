using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> Categories { get; }
        Category Get(int id);
        Category Add(Category category);
        void AddOrUpdateDaypriceForCategory(int categoryId,double price,DateTime date);
        void AddRoom(Room room, int categoryId);
        double GetPriceForDates(int categoryId, DateTime checkInDate, DateTime checkOutDate);
        IEnumerable<Room> GetAllRooms();
        void UpdateRoom(Room room);
        void UpdateCategoryNameAndInfo(int id, string name, string info);
        Image GetImage(int imageId);
        void DeleteImage(int imageId,int categoryId);
        void UpdateImage(Image image,int categoryId);
        void AddImageToCategory(Image image, int categoryId);
        bool HasPriceForAllDays(int categoryId, DateTime checkInDate, DateTime checkOutDate);
    }
}
