using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface ICategoryService
    {
        IEnumerable<CategoryModel> GetAllCategories();
        CategoryModel GetById(int id);
        CategoryModel SaveCategory(CategoryModel categoryModel);
        void UpdateCategoryNameAndDesc(CategoryModel categoryModel);
        ImageModel GetImage(int categoryId, int imageId);
        bool AddOrUpdateCategoryPrice(int categoryId, double price, DateTime startDate, DateTime endDate);
        IEnumerable<RoomModel> GetAllRooms();
        void UpdateRoom(RoomModel roomModel);
        void AddRoom(RoomModel roomModel);
        void DeleteImage(int imageId, int categoryId);
        void UpdateImageInfo(ImageModel imageModel, int categoryId);
        void AddImageToCategory(ImageModel imageModel, int categoryId);
        bool HasPriceForDays(int categoryId,DateTime checkInDate, DateTime checkOutDate);
        double GetPriceForDates(int categoryId, DateTime checkInDate, DateTime checkOutDate);
    }
}
