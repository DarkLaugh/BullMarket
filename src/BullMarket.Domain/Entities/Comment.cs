using BullMarket.Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Domain.Entities
{
    public class Comment : AuditableEntity<Guid>
    {
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
