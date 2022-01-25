using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Catalog.Products.Manage;
using eShop.ViewModels.Catalog.Products.Public;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;

namespace eShop.ApplicationService.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateDto request);
        Task<int> Update(ProductUpdateDto request);

        Task<int> Delete(int productId);
        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task AddViewCount(int productId);

        Task<PagedReadDto<ProductReadDto>> GetAllPaging(GetProductPagingRequest request);

        Task<int> AddImages(int productId, List<IFormFile> files);

        Task<int> RemoveImages(int imageId);
        Task<int> UpdateImages(int imageId, string caption, bool isDefault);

        Task<List<ProductImageReadDto>> GetListImage(int productId);
    }
}
