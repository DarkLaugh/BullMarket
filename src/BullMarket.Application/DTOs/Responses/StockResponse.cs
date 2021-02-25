using System;

namespace BullMarket.Application.DTOs.Responses
{
    public class StockResponse
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public string Status { get; set; }
    }
}
