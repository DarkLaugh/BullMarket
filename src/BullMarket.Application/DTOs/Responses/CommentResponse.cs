using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Application.DTOs.Responses
{
    public class CommentResponse
    {
        public Guid StockId { get; set; }
        public int UserId { get; set; }
        public string CommentContent { get; set; }
    }
}
