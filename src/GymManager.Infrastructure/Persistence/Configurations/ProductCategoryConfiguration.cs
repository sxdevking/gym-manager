using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad ProductCategory
/// </summary>
public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        // Tabla y Schema
        builder.ToTable("ProductCategories", "gym");

        // Clave primaria
        builder.HasKey(pc => pc.CategoryId);

        // Propiedades
        builder.Property(pc => pc.CategoryId)
            .HasColumnName("CategoryId")
            .IsRequired();

        builder.Property(pc => pc.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pc => pc.Description)
            .HasColumnName("Description")
            .HasMaxLength(255);

        builder.Property(pc => pc.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .HasDefaultValue(0);

        // Campos de auditoría
        builder.Property(pc => pc.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(pc => pc.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(pc => pc.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(pc => pc.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(pc => pc.Name)
            .IsUnique()
            .HasDatabaseName("IX_ProductCategories_Name");
    }
}