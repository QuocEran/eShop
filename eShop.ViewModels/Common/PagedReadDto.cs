using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.ViewModels.Common
{
    public class PagedReadDto<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecord { get; set; }
    }
}
