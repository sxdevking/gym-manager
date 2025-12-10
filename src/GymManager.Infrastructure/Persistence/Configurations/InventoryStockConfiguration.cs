using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// INVENTORY STOCK (15_inventory_stock.sql)
// ============================================================
public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.ToTable("inventorystock");

        builder.HasKey(i => i.StockId);

        // C#: Quantity -> SQL: quantity_available
        builder.Property(i => i.Quantity)
            .HasColumnName("quantity_available");

        // C#: RentedQuantity -> SQL: quantity_reserved
        builder.Property(i => i.RentedQuantity)
            .HasColumnName("quantity_reserved");

        // C#: LastRestockDate -> SQL: last_restock_at
        builder.Property(i => i.LastRestockDate)
            .HasColumnName("last_restock_at");

        // Indice unico
        builder.HasIndex(i => new { i.ProductId, i.BranchId }).IsUnique();

        // Relaciones
        builder.HasOne(i => i.Product)
            .WithMany(p => p.InventoryStocks)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Branch)
            .WithMany(b => b.InventoryStocks)
            .HasForeignKey(i => i.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar propiedades de auditoria y calculadas
        builder.Ignore(i => i.DeletedBy);
        builder.Ignore(i => i.IsDeleted);
        builder.Ignore(i => i.DeletedAt);
        builder.Ignore(i => i.CreatedAt);
        builder.Ignore(i => i.CreatedBy);
        builder.Ignore(i => i.AvailableQuantity); // Propiedad calculada

        // Filtro soft delete
        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}