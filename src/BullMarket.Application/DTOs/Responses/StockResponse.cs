using System;
using System.Collections.Generic;

namespace BullMarket.Application.DTOs.Responses
{
    public class StockResponse
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; }
        public string Exchange { get; set; }
        public string Symbol { get; set; }
        public string Status { get; set; }
        public bool Tradable { get; set; }
        public bool Marginable { get; set; }
        public bool Shortable { get; set; }
        public bool EasyToBorrow { get; set; }
        public List<CommentResponse> Comments { get; set; }
    }
}
