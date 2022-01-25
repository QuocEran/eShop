using eShop.ViewModels.Catalog.Products.Public;
using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.ApplicationService.Catalog.Products
{
    public interface IPublicProductService 
    {
        Task<PagedReadDto<ProductReadDto>> GetAllByCategoryId(GetPublicProductPagingRequest request);

    }
}
