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
        void AddRoom(Room room);
        double GetPriceForDates(int categoryId, DateTime checkInDate, DateTime checkOutDate);
        IEnumerable<Room> GetAllRooms();
        void UpdateRoom(Room room);
        void UpdateCategoryNameAndInfo(int id, string name, string info);
        void UpdateCategory(Category category);
        Image GetImage(int imageId);
        void DeleteImage(int imageId,int categoryId);
        void UpdateImageInfo(Image image,int categoryId);
        void AddImageToCategory(Image image, int categoryId);
        bool HasPriceForAllDays(int categoryId, DateTime checkInDate, DateTime checkOutDate);
    }
}
