namespace BullMarket.Application.Common.Models
{
    public class PaginatedListRequest
    {
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }
}
