using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// PRODUCTS (14_products.sql)
// ============================================================
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.ProductId);

        // C#: Sku -> SQL: product_code
        builder.Property(p => p.Sku)
            .HasColumnName("product_code")
            .IsRequired()
            .HasMaxLength(50);

        // C#: Name -> SQL: product_name
        builder.Property(p => p.Name)
            .HasColumnName("product_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasColumnType("text");

        // C#: Price -> SQL: unit_price
        builder.Property(p => p.Price)
            .HasColumnName("unit_price")
            .HasPrecision(10, 2);

        // C#: Cost -> SQL: cost_price
        builder.Property(p => p.Cost)
            .HasColumnName("cost_price")
            .HasPrecision(10, 2);

        builder.Property(p => p.IsRentable)
            .HasColumnName("is_rentable");

        // C#: RentalPrice -> SQL: rental_price_per_day
        builder.Property(p => p.RentalPrice)
            .HasColumnName("rental_price_per_day")
            .HasPrecision(10, 2);

        builder.Property(p => p.RentalDeposit)
            .HasColumnName("rental_deposit")
            .HasPrecision(10, 2);

        // C#: MinStock -> SQL: min_stock_alert
        builder.Property(p => p.MinStock)
            .HasColumnName("min_stock_alert");

        // C#: ImageBase64 -> SQL: image_path
        builder.Property(p => p.ImageBase64)
            .HasColumnName("image_path")
            .HasMaxLength(500);

        // C#: IsAvailable -> SQL: is_active
        builder.Property(p => p.IsAvailable)
            .HasColumnName("is_active");

        // Indices
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasIndex(p => p.CategoryId);

        // Relaciones
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria
        builder.Ignore(p => p.DeletedBy);
        builder.Ignore(p => p.IsDeleted);

        // Filtro soft delete
        builder.HasQueryFilter(p => p.DeletedAt == null);
    }
}