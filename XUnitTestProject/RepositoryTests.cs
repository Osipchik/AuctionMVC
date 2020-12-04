using System;
using System.Threading.Tasks;
using Domain.Core;
using Infrastructure.Data;
using Xunit;

namespace XUnitTestProject
{
    public class RepositoryTests : DataBaseFixture
    {
        private readonly Repository<Lot> _repository;

        public RepositoryTests()
        {
            _repository = new LotRepository(Context);
        }

        [Fact]
        public async Task Find_Test()
        {
            var lot = await _repository.Find(1);

            Assert.Equal(Lots[0].Id, lot.Id);
        }

        [Fact]
        public async Task Create_Test()
        {
            var lot = await _repository.Add(new Lot
            {
                AppUserId = "user_2",
                MinPrice = 10,
                EndAt = DateTime.Now
            });

            Assert.Equal(13, lot.Id);
        }

        [Fact]
        public async Task Delete_Test()
        {
            var lot = await _repository.Find(1);
            var deletedLot = await _repository.Delete(lot);

            Assert.Equal(1, deletedLot.Id);
            Assert.Null(await _repository.Find(1));
        }

        [Fact]
        public async Task Delete_Tes()
        {
            var lot = await _repository.Find(1);

            Assert.Equal("title_0", lot.Title);

            lot.Title = "new title";
            var updatedLot = await _repository.Update(lot);

            Assert.Equal("new title", updatedLot.Title);
        }
    }
}
