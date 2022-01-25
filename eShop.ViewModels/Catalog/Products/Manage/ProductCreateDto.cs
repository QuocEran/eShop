﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.ViewModels.Catalog.Products.Manage
{
    public class ProductCreateDto
    {
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public string Details { set; get; }
        public string SeoDescription { set; get; }
        public string SeoTitle { set; get; }

        public string SeoAlias { get; set; }
        public string LanguageId { set; get; }

        public IFormFile ThumbnailImage { set; get; }
    }
}
