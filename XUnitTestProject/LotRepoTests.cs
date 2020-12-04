using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.SortOptions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace XUnitTestProject
{
    public class LotRepoTests : DataBaseFixture
    {
        private readonly ILotRepository<SortBy, ShowOptions> repository;

        public LotRepoTests()
        {
            repository = new LotRepository(Context);
        }

        [Fact]
        public async Task Find_Rates_Test()
        {
            var lot = await repository.Find(1);
            Assert.Equal(Lots[0], lot);
            Assert.Equal(Lots[0].Id, lot.Id);

            await repository.LoadRates(lot);
            Assert.Equal(Rates, lot.Rates);
            Assert.Equal(Rates.Count, lot.Rates.Count);
        }

        [Fact]
        public async Task Order_Test()
        {
            for (var i = 0; i <= (int)SortBy.DistinctName; i++)
            {
                var query = repository.FilterLots((SortBy)i, ShowOptions.Active, 0);
                var lots = (await repository.FindRange(query, 20, 0)).ToList();

                var expected = SelectLots((SortBy)i, Lots);

                Assert.Equal(expected[0].Id, lots[0].Id);
                Assert.Equal(expected.Last().Id, lots.Last().Id);
                Assert.Equal(expected.Count, lots.Count);
            }
        }

        private List<Lot> SelectLots(SortBy sortBy, List<Lot> Lots)
        {
            var lots = Lots.Where(i => i.IsAvailable && i.EndAt > DateTime.UtcNow);

            lots = sortBy switch
            {
                SortBy.Date => lots.OrderBy(i => i.EndAt),
                SortBy.DistinctDate => lots.OrderByDescending(i => i.EndAt),
                SortBy.Name => lots.OrderBy(i => i.Title),
                SortBy.DistinctName => lots.OrderByDescending(i => i.Title),
                SortBy.Goal => lots.OrderBy(i => i.MinPrice),
                SortBy.Funded => lots.OrderBy(i => i.Rates.OrderByDescending(c => c.CreatedAt).FirstOrDefault().Amount),
                _ => lots
            };

            return lots.ToList();
        }

        [Fact]
        public async Task Filter_Test()
        {
            for (var i = 0; i <= (int)ShowOptions.MyLots; i++)
            {
                var query = repository.FilterLots(SortBy.Date, (ShowOptions)i, 0);
                var lots = (await repository.FindRange(query, 20, 0)).ToList();

                var expected = SelectByActive((ShowOptions)i, 0, Lots);

                Assert.Equal(expected.FirstOrDefault()?.Id, lots.FirstOrDefault()?.Id);
                Assert.Equal(expected.Count, lots.Count);
            }
        }

        [Fact]
        public async Task Filter_category_Test()
        {
            for (var i = 0; i <= (int)ShowOptions.MyLots; i++)
            {
                var query = repository.FilterLots(SortBy.Date, (ShowOptions)i, 1);
                var lots = (await repository.FindRange(query, 20, 0)).ToList();

                var expected = SelectByActive((ShowOptions)i, 1, Lots);

                Assert.Equal(expected.FirstOrDefault()?.Id, lots.FirstOrDefault()?.Id);
                Assert.Equal(expected.Count, lots.Count);
            }
        }

        private List<Lot> SelectByActive(ShowOptions show, int categoryId, List<Lot> Lots)
        {
            IEnumerable<Lot> expected;

            if (categoryId != 0)
            {
                expected = show switch
                {
                    ShowOptions.Active => Lots.Where(i => i.IsAvailable && i.EndAt > DateTime.UtcNow && i.CategoryId == categoryId),
                    ShowOptions.Sold => Lots.Where(i => i.IsAvailable && i.EndAt < DateTime.UtcNow && i.CategoryId == categoryId),
                    ShowOptions.All => Lots.Where(i => i.IsAvailable && i.CategoryId == categoryId),
                    _ => Lots.Where(i => i.CategoryId == categoryId)
                };
            }
            else
            {
                expected = show switch
                {
                    ShowOptions.Active => Lots.Where(i => i.IsAvailable && i.EndAt > DateTime.UtcNow),
                    ShowOptions.Sold => Lots.Where(i => i.IsAvailable && i.EndAt < DateTime.UtcNow),
                    ShowOptions.All => Lots.Where(i => i.IsAvailable),
                    _ => Lots
                };
            }

            return expected.OrderBy(i => i.EndAt).ToList();
        }
    }
}
