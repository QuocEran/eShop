using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.ViewModels.Catalog.Products.Public
{
    public class ProductImageReadDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public bool IsDefault { get; set; }
        public long FileSize { get; set; }
    }
}
