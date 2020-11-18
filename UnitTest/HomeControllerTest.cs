using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Domain.Interfaces;
using Infrastructure.Data.SortOptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Web.DTO;
using Xunit;

namespace UnitTest
{
    public class HomeControllerTest
    {
        private Mock<ICategoryRepository> mockCategoryRepo;
        private Mock<ILotRepository<SortBy, ShowOptions>> mockLotRepo;
        private IEnumerable<Category> categoryList;
        private IEnumerable<Lot> LotsList;
        
        public HomeControllerTest()
        {
            categoryList = GetCategories();
            LotsList = GetLots();
            
            mockCategoryRepo = new Mock<ICategoryRepository>();
            mockCategoryRepo.Setup(repo => repo.GetAll())
                .ReturnsAsync(categoryList);
            
            mockLotRepo = new Mock<ILotRepository<SortBy, ShowOptions>>();
            mockLotRepo.Setup(i => i.FilterLots(SortBy.Date, ShowOptions.Active, 0))
                .Returns(LotQuery());
        }
        
        [Fact]
        public async Task HomeIndex()
        {
            var controller = new HomeController(mockLotRepo.Object, mockCategoryRepo.Object);

            var result = await controller.Index("", SortBy.Date, ShowOptions.All, 0);

            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public async Task HomeLoadLots()
        {
            var controller = new HomeController(mockLotRepo.Object, mockCategoryRepo.Object);

            var result = await controller.LoadLots("", 0, SortBy.Date, ShowOptions.Active, 20, 0);

            Assert.IsType<PartialViewResult>(result);
        }
        
        [Fact]
        public void HomeNotFoundError()
        {
            var controller = new HomeController(mockLotRepo.Object, mockCategoryRepo.Object);

            var result = controller.NotFoundError(new NotfoundErrorViewModel());

            Assert.IsType<ViewResult>(result);
        }
        
        private IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>();
            for (var i = 0; i < 5; i++)
            {
                categories.Add(new Category {Id = i});
            }
            
            return categories;
        }

        private IEnumerable<Lot> GetLots()
        {
            var lots = new List<Lot>();

            for (var i = 0; i < 20; i++)
            {
                lots.Add(new Lot
                {
                    Id = i,
                    AppUserId = "user_1",
                    AppUser = GetUser(1),
                    CategoryId = 1,
                    EndAt = DateTime.Now.AddDays(1),
                    CreatedAt = DateTime.Now,
                    Title = "title_" + i,
                    IsAvailable = true
                });
            }
            
            for (var i = 20; i < 40; i++)
            {
                lots.Add(new Lot
                {
                    Id = i,
                    AppUserId = "user_2",
                    AppUser = GetUser(2),
                    CategoryId = 2,
                    EndAt = DateTime.Now.AddDays(1),
                    CreatedAt = DateTime.Now,
                    Title = "title_" + i,
                    IsAvailable = false
                });
            }

            return lots;
        }

        private AppUser GetUser(int id)
        {
            var user = new AppUser
            {
                Id = "user_" + id,
                UserName = "user_" + id
            };

            return user;
        }
        
        private IQueryable<Lot> LotQuery()
        {
            var query = LotsList.AsQueryable();

            return query;
        }
    }
}