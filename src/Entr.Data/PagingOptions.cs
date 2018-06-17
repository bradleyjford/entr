using System.Collections.Generic;

namespace Entr.Data
{
    public interface IPagingOptions
    {
        int PageNumber { get; }
        int PageSize { get; }

        IEnumerable<SortDescriptor> SortDescriptors { get; }
    }

    public class PagingOptions : IPagingOptions
    {
        const int DefaultPageSize = 50;

        int _pageNumber = 1;
        int _pageSize = DefaultPageSize;

        public string SortOrder { get; set; }

        public IEnumerable<SortDescriptor> SortDescriptors => SortDescriptorParser.Parse(SortOrder);

        public int PageNumber
        {
            get => _pageNumber;

            set
            {
                _pageNumber = value;

                if (_pageNumber <= 0)
                {
                    _pageNumber = 1;
                }
            }
        }
        
        public int PageSize
        {
            get => _pageSize;

            set 
            { 
                _pageSize = value;
            
                if (_pageSize <= 0)
                {
                    _pageSize = DefaultPageSize;
                }
            }
        }
    }
}
