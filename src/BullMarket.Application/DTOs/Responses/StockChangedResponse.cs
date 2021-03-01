using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Application.DTOs.Responses
{
    public class StockChangedResponse
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
    }
}
