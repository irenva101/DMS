using System.ComponentModel;

namespace DMS.Shared.Commons
{
    public class PageInfo
    {
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        public PageInfo()
        {

        }
        public PageInfo(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}
