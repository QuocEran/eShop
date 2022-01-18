using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.HasKey(t => new { t.CategoryId, t.ProductId });
            builder.ToTable("ProductInCategories");

            builder.HasOne(t => t.Product).WithMany(p => p.ProductInCategories)
                .HasForeignKey(t => t.ProductId);

            builder.HasOne(t => t.Category).WithMany(p => p.ProductInCategories)
                .HasForeignKey(t => t.CategoryId);

        }
    }
}
