using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// PRODUCT CATEGORIES (13_product_categories.sql)
// ============================================================
public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("productcategories");

        builder.HasKey(pc => pc.CategoryId);

        // C#: Name -> SQL: category_name
        builder.Property(pc => pc.Name)
            .HasColumnName("category_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pc => pc.Description)
            .HasMaxLength(255);

        // C#: DisplayOrder -> SQL: sort_order
        builder.Property(pc => pc.DisplayOrder)
            .HasColumnName("sort_order");

        // Indices
        builder.HasIndex(pc => pc.Name).IsUnique();

        // Ignorar propiedades de auditoria
        builder.Ignore(pc => pc.DeletedBy);
        builder.Ignore(pc => pc.IsDeleted);
        builder.Ignore(pc => pc.DeletedAt);
        builder.Ignore(pc => pc.UpdatedAt);
        builder.Ignore(pc => pc.UpdatedBy);
        builder.Ignore(pc => pc.CreatedBy);
    }
}