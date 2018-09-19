
using System.Collections.Generic;

namespace GildedRose
{
    public class Item
    {
        public int SellIn { get; set; }
        public int Quality { get; set; }
        public bool IsConjured { get; set; }
    }

    public class Inventory
    {
        public List<Item> Items;
        public Item AgedBrie;
        public Item Sulfuras;
        public Item BackstagePass;

        public void UpdateQuality()
        {

        }
    }
}
