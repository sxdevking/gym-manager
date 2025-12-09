using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad InventoryStock
/// </summary>
public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        // Tabla y Schema
        builder.ToTable("InventoryStock", "gym");

        // Clave primaria
        builder.HasKey(i => i.StockId);

        // Propiedades
        builder.Property(i => i.StockId)
            .HasColumnName("StockId")
            .IsRequired();

        builder.Property(i => i.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(i => i.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("Quantity")
            .HasDefaultValue(0);

        builder.Property(i => i.RentedQuantity)
            .HasColumnName("RentedQuantity")
            .HasDefaultValue(0);

        builder.Property(i => i.LastRestockDate)
            .HasColumnName("LastRestockDate");

        // Ignorar propiedades calculadas
        builder.Ignore(i => i.AvailableQuantity);

        // Campos de auditoría
        builder.Property(i => i.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(i => i.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(i => i.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(i => new { i.ProductId, i.BranchId })
            .IsUnique()
            .HasDatabaseName("IX_InventoryStock_ProductId_BranchId");

        // Relaciones
        builder.HasOne(i => i.Product)
            .WithMany(p => p.InventoryStocks)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Branch)
            .WithMany(b => b.InventoryStocks)
            .HasForeignKey(i => i.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}