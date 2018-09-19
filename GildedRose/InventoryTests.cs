using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GildedRose.Tests
{
    public class GildedRoseInventoryTests
    {
        public GildedRose.Inventory Inventory;

        public GildedRoseInventoryTests()
        {
            Inventory = new Inventory();
        }

        [Fact]
        public void TestInventoryHasItems()
        {
            Assert.NotNull(Inventory.Items);
        }

        [Fact]
        public void TestAgedBrieExists()
        {
            Assert.NotNull(Inventory.AgedBrie);
        }

        [Fact]
        public void TestSulfurasExists()
        {
            Assert.NotNull(Inventory.Sulfuras);
        }

        [Fact]
        public void TestBackstagePassExists()
        {
            Assert.NotNull(Inventory.BackstagePass);
        }

        [Fact]
        public void TestItemSellInStartsNonNegative()
        {
            foreach (var item in Inventory.Items)
            {
                Assert.True(item.SellIn >= 0);
            }
        }

        [Fact]
        public void TestItemSellInDecreases()
        {
            var sulfuras = Inventory.Sulfuras;
            var items = Inventory.Items.Where(i => i != sulfuras);

            Dictionary<Item, int> oldSellInValues = new Dictionary<Item, int>();
            foreach (var item in items)
            {
                oldSellInValues[item] = item.SellIn;
            }

            Inventory.UpdateQuality();
            foreach (var item in items)
            {
                Assert.True(oldSellInValues[item] > item.SellIn);
            }
        }

        [Fact]
        public void TestItemQualityStartsNotNegative()
        {
            foreach (var item in Inventory.Items)
            {
                Assert.True(item.Quality >= 0);
            }
        }

        [Fact]
        public void TestItemQualityStaysNotNegative()
        {
            Inventory.UpdateQuality();

            foreach (var item in Inventory.Items)
            {
                Assert.True(item.Quality >= 0);
            }
        }

        [Fact]
        public void TestItemQualityStartsNoMoreThan50()
        {
            foreach (var item in Inventory.Items)
            {
                Assert.True(item.Quality <= 50 || item == Inventory.Sulfuras);
            }
        }

        [Fact]
        public void TestItemQualityStaysNoMoreThan50()
        {
            Inventory.UpdateQuality();

            foreach (var item in Inventory.Items)
            {
                Assert.True(item.Quality <= 50 || item == Inventory.Sulfuras);
            }
        }

        [Fact]
        public void TestAgedBrieQualityIncreases()
        {
            var agedBrie = Inventory.AgedBrie;
            var oldQuality = agedBrie.Quality;

            Inventory.UpdateQuality();
            Assert.True(agedBrie.Quality > oldQuality);
        }

        [Fact]
        public void TestSulfurasQualityDoesNotChange()
        {
            var sulfuras = Inventory.Sulfuras;
            var oldQuality = sulfuras.Quality;

            Inventory.UpdateQuality();
            Assert.Equal(oldQuality, sulfuras.Quality);
        }

        [Fact]
        public void TestSulfurasSellInIsZero()
        {
            var sulfuras = Inventory.Sulfuras;

            Assert.Equal(0, sulfuras.SellIn);
            Inventory.UpdateQuality();
            Assert.Equal(0, sulfuras.SellIn);
        }

        [Fact]
        public void TestBackstagePassQualityIncreasesBeforeSellIn()
        {
            var backstagePass = Inventory.BackstagePass;
            var oldQuality = backstagePass.Quality;

            backstagePass.SellIn = 20;
            Inventory.UpdateQuality();
            Assert.True(backstagePass.Quality > oldQuality);
        }

        [Fact]
        public void TestBackstagePassQualityIncreasesBy2IfSellInLessThan10()
        {
            var backstagePass = Inventory.BackstagePass;
            var oldQuality = backstagePass.Quality;

            backstagePass.SellIn = 10;
            Inventory.UpdateQuality();
            Assert.Equal(oldQuality + 2, backstagePass.Quality);
        }

        [Fact]
        public void TestBackstagePassQualityIncreasesBy3IfSellInLessThan5()
        {
            var backstagePass = Inventory.BackstagePass;
            var oldQuality = backstagePass.Quality;

            backstagePass.SellIn = 5;
            Inventory.UpdateQuality();
            Assert.Equal(oldQuality + 3, backstagePass.Quality);
        }

        [Fact]
        public void TestBackstagePassQualityIsZeroAfterSellIn()
        {
            var backstagePass = Inventory.BackstagePass;
            var oldQuality = backstagePass.Quality;

            backstagePass.SellIn = 1;
            Inventory.UpdateQuality();
            Assert.Equal(0, backstagePass.Quality);
            Inventory.UpdateQuality();
            Assert.Equal(0, backstagePass.Quality);
        }

        [Fact]
        public void TestConjuredItemsExist()
        {
            var conjuredItems = Inventory.Items.Where(i => i.IsConjured);
            Assert.NotEmpty(conjuredItems);
        }

        [Fact]
        public void TestConjuredItemsDegradeAtDoubleRate()
        {
            var conjuredItem = Inventory.Items.First(i => i.IsConjured);
            var normalItem = Inventory.Items.First(
                i => !i.IsConjured &&
                i != Inventory.AgedBrie &&
                i != Inventory.Sulfuras &&
                i != Inventory.BackstagePass);

            var oldConjuredQuality = conjuredItem.Quality;
            var oldNormalQuality = normalItem.Quality;
            Inventory.UpdateQuality();
            var conjuredQualityDiff = oldConjuredQuality - conjuredItem.Quality;
            var normalQualityDiff = oldNormalQuality - normalItem.Quality;

            Assert.Equal(normalQualityDiff * 2, conjuredQualityDiff);
        }
    }
}