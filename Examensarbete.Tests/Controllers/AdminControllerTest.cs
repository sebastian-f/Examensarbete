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

namespace Examensarbete.Tests.Controllers
{
    [TestClass]
    public class AdminControllerTest
    {
        [TestMethod]
        public void TestIfIndexMethodReturnsModelWithCategories()
        {
            // Arrange    
            Mock<ICategoryRepository> mock = new Mock<ICategoryRepository>();
            mock.Setup(m => m.Categories).Returns(new Category[] {        
                new Category {Id = 1, Name = "Dubbelrum"},        
                new Category {Id = 2, Name = "Enkelrum"},        
                new Category {Id = 3, Name = "Svit"},        
                new Category {Id = 4, Name = "Familjerum"}    });

            // Arrange    
            AdminController controller = new AdminController(mock.Object);

            // Act   
            ViewResult result = controller.Index() as ViewResult;

            // Assert    
            IEnumerable<Category> categories = result.Model as IEnumerable<Category>;
            Assert.AreEqual(categories.Count(), 4);
        }

    }
}