using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Examensarbete;
using Examensarbete.Controllers;
using Domain.Abstract;
using Moq;
using Domain.Entities;
using Service.Interface;

namespace Examensarbete.Tests.Controllers
{
    [TestClass]
    public class AdminControllerTest
    {
        [TestMethod]
        public void TestIfIndexMethodReturnsModelWithCategories()
        {
            // Arrange    
            Mock<ICategoryService> mockService = new Mock<ICategoryService>();
            Mock<ICategoryRepository> mock = new Mock<ICategoryRepository>();
            mock.Setup(m => m.Categories).Returns(new Category[] {        
                new Category {Id = 1, Name = "Dubbelrum"},        
                new Category {Id = 2, Name = "Enkelrum"},        
                new Category {Id = 3, Name = "Svit"},        
                new Category {Id = 4, Name = "Familjerum"}    });

            // Arrange    
            AdminController controller = new AdminController(mockService.Object);

            // Act   
            ViewResult result = controller.Categories() as ViewResult;

            // Assert    
            IEnumerable<Category> categories = result.Model as IEnumerable<Category>;
            Assert.AreEqual(categories.Count(), 4);
        }

    }
}