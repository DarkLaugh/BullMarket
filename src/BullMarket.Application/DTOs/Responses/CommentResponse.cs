using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Application.DTOs.Responses
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
