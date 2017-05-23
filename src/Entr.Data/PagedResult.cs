using System;
using System.Collections.Generic;
using System.Linq;

namespace Entr.Data
{
    public interface IPagedResult
    {
        int PageNumber { get; }
        int PageSize { get; }
        int ItemCount { get; }
        int PageCount { get; }
        IEnumerable<object> Items { get; }
    }

    public interface IPagedResult<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        int ItemCount { get; }
        int PageCount { get; }
        IEnumerable<T> Items { get; }
    }

    public class PagedResult<T> : IPagedResult<T>, IPagedResult
    {
        public PagedResult(int pageNumber, int pageSize, IEnumerable<T> items, int itemCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            ItemCount = itemCount;
            Items = items;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int ItemCount { get; private set; }
        public IEnumerable<T> Items { get; private set; }

        public int PageCount
        {
            get { return (int)Math.Ceiling(((double)ItemCount / PageSize)); }
        }

        IEnumerable<object> IPagedResult.Items
        {
            get { return Items.Cast<object>(); }
        }
    }

    public class PagedResult : PagedResult<object>
    {
        public PagedResult(int pageNumber, int pageSize, IEnumerable<object> items, int itemCount) 
            : base(pageNumber, pageSize, items, itemCount)
        {
        }
    }
}
