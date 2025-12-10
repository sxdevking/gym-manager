using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// SALES (16_sales.sql)
// ============================================================
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.SaleId);

        builder.Property(s => s.TicketNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(s => s.SaleDate)
            .IsRequired();

        builder.Property(s => s.Subtotal)
            .HasPrecision(10, 2);

        builder.Property(s => s.DiscountAmount)
            .HasPrecision(10, 2);

        builder.Property(s => s.TaxAmount)
            .HasPrecision(10, 2);

        // C#: Total -> SQL: total_amount
        builder.Property(s => s.Total)
            .HasColumnName("total_amount")
            .HasPrecision(10, 2);

        // Enum Status
        builder.Property(s => s.Status)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<SaleStatus>(v, true))
            .HasMaxLength(20);

        // C#: PaymentReference -> SQL no existe, ignorar
        builder.Ignore(s => s.PaymentReference);

        builder.Property(s => s.Notes)
            .HasColumnType("text");

        // C#: UserId -> SQL: created_by (el usuario que creo la venta)
        builder.Property(s => s.UserId)
            .HasColumnName("created_by");

        // C#: PaymentMethodId -> SQL no tiene directamente
        // Las ventas pueden tener multiples pagos via tabla Payments
        // Ignoramos esta propiedad o la mapeamos a una columna auxiliar
        builder.Ignore(s => s.PaymentMethodId);

        // Indices
        builder.HasIndex(s => s.TicketNumber).IsUnique();
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.MemberId);
        builder.HasIndex(s => s.SaleDate);
        builder.HasIndex(s => s.Status);

        // Relaciones
        builder.HasOne(s => s.Branch)
            .WithMany(b => b.Sales)
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Member)
            .WithMany(m => m.Sales)
            .HasForeignKey(s => s.MemberId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignoramos PaymentMethod porque no hay FK directa en SQL
        builder.Ignore(s => s.PaymentMethod);

        // Ignorar propiedades de auditoria
        builder.Ignore(s => s.DeletedBy);
        builder.Ignore(s => s.IsDeleted);
        builder.Ignore(s => s.DeletedAt);
        builder.Ignore(s => s.UpdatedAt);
        builder.Ignore(s => s.UpdatedBy);
        builder.Ignore(s => s.CreatedBy); // Ya mapeado a UserId
        builder.Ignore(s => s.CreatedAt); // SaleDate es la fecha

        // Filtro soft delete
        builder.HasQueryFilter(s => s.DeletedAt == null);
    }
}