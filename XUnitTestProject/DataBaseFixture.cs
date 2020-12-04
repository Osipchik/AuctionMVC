using Domain.Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestProject
{
    public class DataBaseFixture
    {
        protected AppDbContext Context;
        protected List<Category> Categories;
        protected List<Lot> Lots;
        protected List<Comment> Comments;
        protected List<Rate> Rates;

        protected DbContextOptions<AppDbContext> dbContextOptions;

        public DataBaseFixture()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuctionTestDB")
                .Options;

            Init();
        }

        private void Init()
        {
            Context = new AppDbContext(dbContextOptions);

            Categories = InitCategories();
            Lots = InitLots();
            Comments = InitComments();
            Rates = InitRates();

            Context.SaveChanges();
        }

        private List<Category> InitCategories()
        {
            var categories = new List<Category>
            {
                new Category{ Name = "category_1" },
                new Category{ Name = "category_2" }
            };

            Context.Categories.AddRange(categories);

            return categories;
        }

        private List<Lot> InitLots()
        {
            var lots = new List<Lot>();

            for (var i = 0; i < 11; i++)
            {
                lots.Add(new Lot
                {
                    Title = "title_" + i,
                    CategoryId = 1,
                    CreatedAt = DateTime.Now.AddSeconds(i),
                    LunchAt = DateTime.Now.AddDays(1 + i),
                    EndAt = DateTime.Now.AddDays(2 + i),
                    MinPrice = i * 100,
                    IsAvailable = i < 5,
                    AppUserId = i < 5 ? "user_1" : "user_2"
                });
            }

            lots.Add(new Lot
            {
                Title = "title_" + 11,
                CategoryId = 2,
                CreatedAt = DateTime.Now.AddSeconds(11),
                LunchAt = DateTime.Now.AddDays(12),
                EndAt = DateTime.Now.AddDays(13),
                MinPrice = 11 * 100,
                IsAvailable = true,
                AppUserId = "user_2"
            });


            Context.Lots.AddRange(lots);

            return lots;
        }

        private List<Comment> InitComments()
        {
            var comments = new List<Comment>();

            for (var i = 0; i < 10; i++)
            {
                comments.Add(new Comment
                {
                    AppUserId = "user_1",
                    CreatedAt = DateTime.Now,
                    LotId = 1,
                    Text = "1"
                });
            }

            Context.Comments.AddRange(comments);

            return comments;
        }

        private List<Rate> InitRates()
        {
            var rates = new List<Rate>();

            for (var i = 1; i < 11; i++)
            {
                rates.Add(new Rate
                {
                    Amount = i * 10,
                    AppUserId = "user_1",
                    CreatedAt = DateTime.Now,
                    LotId = 1
                });
            }

            Context.Rates.AddRange(rates);

            return rates;
        }
    }
}
