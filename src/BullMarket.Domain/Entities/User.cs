using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BullMarket.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public List<Comment> Comments { get; set; }
    }
}
