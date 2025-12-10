using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// SALE ITEMS (17_sale_items.sql)
// ============================================================
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("saleitems");

        builder.HasKey(si => si.SaleItemId);

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasPrecision(10, 2);

        // C#: Discount -> SQL: discount_amount
        builder.Property(si => si.Discount)
            .HasColumnName("discount_amount")
            .HasPrecision(10, 2);

        // C#: Subtotal -> SQL: line_total
        builder.Property(si => si.Subtotal)
            .HasColumnName("line_total")
            .HasPrecision(10, 2);

        builder.Property(si => si.IsRental)
            .HasColumnName("is_rental");

        // C#: RentalReturnDate -> SQL: rental_expected_return
        builder.Property(si => si.RentalReturnDate)
            .HasColumnName("rental_expected_return");

        // C#: ActualReturnDate -> SQL: rental_actual_return
        builder.Property(si => si.ActualReturnDate)
            .HasColumnName("rental_actual_return");

        // C#: DepositAmount -> SQL: rental_deposit_paid
        builder.Property(si => si.DepositAmount)
            .HasColumnName("rental_deposit_paid")
            .HasPrecision(10, 2);

        // C#: DepositReturned -> SQL: rental_deposit_returned (es decimal, no bool)
        // Ignoramos porque los tipos no coinciden
        builder.Ignore(si => si.DepositReturned);

        // Indices
        builder.HasIndex(si => si.SaleId);
        builder.HasIndex(si => si.ProductId);
        builder.HasIndex(si => si.IsRental);

        // Relaciones
        builder.HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria y calculadas
        builder.Ignore(si => si.DeletedBy);
        builder.Ignore(si => si.IsDeleted);
        builder.Ignore(si => si.DeletedAt);
        builder.Ignore(si => si.UpdatedAt);
        builder.Ignore(si => si.UpdatedBy);
        builder.Ignore(si => si.CreatedAt);
        builder.Ignore(si => si.CreatedBy);
        builder.Ignore(si => si.IsPendingReturn); // Propiedad calculada

        // Filtro soft delete
        builder.HasQueryFilter(si => si.DeletedAt == null);
    }
}
