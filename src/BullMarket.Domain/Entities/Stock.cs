using BullMarket.Domain.Common;
using System;

namespace BullMarket.Domain.Entities
{
    public class Stock : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public float CurrentPrice { get; set; }
        public float PreviousPrice { get; set; }
        public int QuantityAvailable { get; set; }

        public Stock(string name, string abbreviation, float currentPrice, float previousPrice, int quantityAvailable)
        {
            Name = name;
            Abbreviation = abbreviation;
            CurrentPrice = currentPrice;
            PreviousPrice = previousPrice;
            QuantityAvailable = quantityAvailable;
        }
    }
}
