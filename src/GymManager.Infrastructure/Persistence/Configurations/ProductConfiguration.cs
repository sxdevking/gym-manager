using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Product
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Tabla y Schema
        builder.ToTable("Products", "gym");

        // Clave primaria
        builder.HasKey(p => p.ProductId);

        // Propiedades
        builder.Property(p => p.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .HasColumnName("CategoryId")
            .IsRequired();

        builder.Property(p => p.Sku)
            .HasColumnName("Sku")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasColumnName("Price")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.Cost)
            .HasColumnName("Cost")
            .HasPrecision(10, 2);

        builder.Property(p => p.IsRentable)
            .HasColumnName("IsRentable")
            .HasDefaultValue(false);

        builder.Property(p => p.RentalPrice)
            .HasColumnName("RentalPrice")
            .HasPrecision(10, 2);

        builder.Property(p => p.RentalDeposit)
            .HasColumnName("RentalDeposit")
            .HasPrecision(10, 2);

        builder.Property(p => p.MinStock)
            .HasColumnName("MinStock")
            .HasDefaultValue(5);

        builder.Property(p => p.ImageBase64)
            .HasColumnName("ImageBase64");

        builder.Property(p => p.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasDefaultValue(true);

        // Campos de auditoría
        builder.Property(p => p.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(p => p.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasDatabaseName("IX_Products_Sku");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        // Relaciones
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}