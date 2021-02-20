using System;

namespace BullMarket.Domain.Common
{
    public class AuditableEntity<T> : BaseEntity<T>
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public  DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
