using BullMarket.Domain.Common;
using System;
using System.Collections.Generic;

namespace BullMarket.Domain.Entities
{
    public class Stock : BaseEntity<Guid>
    {
        public string ClassName { get; set; }
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public string Status { get; set; }
        public bool Tradable { get; set; }
        public bool Marginable { get; set; }
        public bool Shortable { get; set; }
        public bool EasyToBorrow { get; set; }
        public List<Comment> Comments { get; set; }


        public Stock(string className, string exchange, string symbol, string status,
            bool tradable, bool marginable, bool shortable, bool easyToBorrow)
        {
            ClassName = className;
            Exchange = exchange;
            Symbol = symbol;
            Status = status;
            Tradable = tradable;
            Marginable = marginable;
            Shortable = shortable;
            EasyToBorrow = easyToBorrow;
        }

        public Stock()
        {

        }
    }
}
