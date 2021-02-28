using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Application.DTOs.Requests
{
    public class CommentRequest
    {
        public Guid StockId { get; set; }
        public string CommentContent { get; set; }
    }
}
