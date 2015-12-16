using Domain.Abstract;
using Domain.Entities;
using Service.DTO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;


namespace Service.Implementation
{
    public class CategoryService : ICategoryService
    {
        ICategoryRepository categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public IEnumerable<CategoryModel> GetAllCategories()
        {
            IEnumerable<Category> categoryEntities = categoryRepository.Categories;
            IEnumerable<CategoryModel> categories = Mapper.Map<IEnumerable<Category>, IEnumerable<CategoryModel>>(categoryEntities);
            return categories;
        }


        public CategoryModel GetById(int id)
        {
            Category categoryEntity = categoryRepository.Get(id);
            CategoryModel categoryModel = Mapper.Map<Category, CategoryModel>(categoryEntity);
            return categoryModel;
        }


        public CategoryModel SaveCategory(CategoryModel categoryModel)
        {
            Category category = Mapper.Map<CategoryModel, Category>(categoryModel);
            Category categoryToReturn = categoryRepository.Add(category);
            CategoryModel categoryModelToReturn = Mapper.Map<Category, CategoryModel>(categoryToReturn);

            return categoryModelToReturn;
        }


        public void UpdateCategoryNameAndDesc(CategoryModel categoryModel)
        {
            Category category = categoryRepository.Get(categoryModel.Id);
            category.Name = categoryModel.Name;
            category.Description = categoryModel.Description;
            categoryRepository.UpdateCategory(category);
        }


        public ImageModel GetImage(int categoryId, int imageId)
        {
            Category category;
            Image image;
            try
            {
                category = categoryRepository.Get(categoryId);
                image = category.Images.Where(img => img.Id == imageId).FirstOrDefault();
            }
            catch 
            {
                return null;
            }
            ImageModel imageModel = Mapper.Map<Image, ImageModel>(image);
            return imageModel;
        }


        public bool AddOrUpdateCategoryPrice(int categoryId, double price, DateTime startDate, DateTime endDate)
        {
            //TODO: Check if the difference between startDate and endDate aren't to big. 100 years means a LOT of memory!
            if (startDate > endDate) return false;
            if (categoryRepository.Get(categoryId) == null) return false;

            DateTime currentDate = startDate;
            while(currentDate<=endDate)
            {
                 categoryRepository.AddOrUpdateDaypriceForCategory(categoryId, price, currentDate);
                 currentDate = currentDate.AddDays(1);
            }

            return true;
        }


        public IEnumerable<RoomModel> GetAllRooms()
        {
            IEnumerable<Room> rooms = categoryRepository.GetAllRooms();
            IEnumerable<RoomModel> roomModels = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomModel>>(rooms);
            return roomModels;
        }


        public void UpdateRoom(RoomModel roomModel)
        {
            Room room = Mapper.Map<RoomModel, Room>(roomModel);
            categoryRepository.UpdateRoom(room);
        }


        public void AddRoom(RoomModel roomModel)
        {
            Room room = Mapper.Map<RoomModel, Room>(roomModel);
            categoryRepository.AddRoom(room);
        }

        public void DeleteImage(int imageId, int categoryId)
        {
            categoryRepository.DeleteImage(imageId, categoryId);
        }


        public void UpdateImageInfo(ImageModel imageModel, int categoryId)
        {
            Image image = Mapper.Map<ImageModel,Image>(imageModel);
            categoryRepository.UpdateImageInfo(image, categoryId);
        }


        public void AddImageToCategory(ImageModel imageModel, int categoryId)
        {
            Image image = Mapper.Map<ImageModel, Image>(imageModel);
            categoryRepository.AddImageToCategory(image, categoryId);
        }


        public bool HasPriceForDays(int categoryId, DateTime checkInDate, DateTime checkOutDate)
        {
            return categoryRepository.HasPriceForAllDays(categoryId,checkInDate,checkOutDate);
        }


        public double GetPriceForDates(int categoryId, DateTime checkInDate, DateTime checkOutDate)
        {
            return categoryRepository.GetPriceForDates(categoryId, checkInDate, checkOutDate);
        }
    }
}
