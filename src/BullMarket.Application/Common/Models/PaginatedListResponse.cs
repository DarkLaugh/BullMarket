using System;
using System.Collections.Generic;

namespace BullMarket.Application.Common.Models
{
    public class PaginatedListResponse<T> 
        where T : class
    {
        public IEnumerable<T> Items { get; }
        public int PageIndex { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedListResponse(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public PaginatedListResponse()
        {

        }
    }
}
