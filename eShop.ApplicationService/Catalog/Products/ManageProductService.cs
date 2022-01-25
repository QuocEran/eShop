using eShop.ApplicationService.Common;
using eShop.Data.EF;
using eShop.Data.Models;
using eShop.Utilities.Exceptions;
using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Catalog.Products.Manage;
using eShop.ViewModels.Catalog.Products.Public;
using eShop.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace eShop.ApplicationService.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly eShopDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public ManageProductService(eShopDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public Task<int> AddImages(int productId, List<IFormFile> files)
        {
            throw new NotImplementedException();
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateDto request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Details,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    }
                }
            };
            //Save Image
            if(request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>()
                {
                    new ProductImage()
                    {
                        Caption = "Thumbnail Image",
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1,
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length
                    }
                };
            }

            _context.Products.Add(product);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new eShopException("Can't find product!: " + productId);
            }

            // Delete file in disk

            var images = _context.ProductImages.Where(i => i.ProductId == productId);

            if (images != null)
            {
                foreach( var image in images)
                {
                    await _fileStorageService.DeleteFileAysnc(image.ImagePath);
                }
            }
               

            // Delete in DB
            _context.Products.Remove(product);
           
            return await _context.SaveChangesAsync();
        }


        public async Task<PagedReadDto<ProductReadDto>> GetAllPaging(GetProductPagingRequest request)
        {
            #region 1. Select join
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };
            #endregion
            #region 2. Filter
            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.pt.Name.Contains(request.Keyword));

            if(request.CategoryId.Count > 0)
            {
                query = query.Where(p => request.CategoryId.Contains(p.pic.CategoryId));
            }
            #endregion
            #region 3. Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1)*request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductReadDto()
                {
                    Id = x.p.Id,
                    Name = x.pt.Name,
                    DateCreated = x.p.DateCreated,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    LanguageId = x.pt.LanguageId,
                    OriginalPrice = x.p.OriginalPrice,
                    Price = x.p.Price,
                    SeoAlias = x.pt.SeoAlias,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                }).ToListAsync();
            #endregion
            #region 4. Select and projection
            var pagedResult = new PagedReadDto<ProductReadDto>()
            {
                TotalRecord = totalRow,
                Items = data
            };
            #endregion

            // Return
            return pagedResult;
        }

        public Task<List<ProductImageReadDto>> GetListImage(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveImages(int imageId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(ProductUpdateDto request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            
            var productTranslations = await _context.ProductTranslations.
                FirstOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);

            if (product == null || productTranslations == null)
                throw new eShopException("Cannot find product with Id: " + request.Id);
            else
            {
                productTranslations.Name = request.Name;
                productTranslations.Description = request.Description;
                productTranslations.SeoAlias = request.SeoAlias;
                productTranslations.SeoDescription = request.SeoDescription;
                productTranslations.SeoTitle = request.SeoTitle;
                productTranslations.Details = request.Details;
            }
            //Update Image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = await _context.ProductImages
                    .FirstOrDefaultAsync(i => i.IsDefault == true && i.ProductId == request.Id);

                if (thumbnailImage != null)
                {
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    thumbnailImage.DateCreated = DateTime.Now;
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;

                    _context.ProductImages.Update(thumbnailImage);
                }

                //else
                //{
                //    product.ProductImages = new List<ProductImage>()
                //    {
                //        new ProductImage()
                //        {
                //            Caption = "Thumbnail Image",
                //            ImagePath = await this.SaveFile(request.ThumbnailImage),
                //            IsDefault = true,
                //            SortOrder = 1,
                //            DateCreated = DateTime.Now,
                //            FileSize = request.ThumbnailImage.Length
                //        }
                //    };
                //}
            }
            return await _context.SaveChangesAsync();
        }

        public Task<int> UpdateImages(int imageId, string caption, bool isDefault)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new eShopException("Cannot find product with Id: " + productId);
            else
            {
                product.Price = newPrice;
            }
            return (await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new eShopException("Cannot find product with Id: " + productId);
            else
            {
                product.Stock += addedQuantity;
            }
            return (await _context.SaveChangesAsync() > 0);
        }


        // Save Image private method
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _fileStorageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}