using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Sale
/// </summary>
public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        // Tabla y Schema
        builder.ToTable("Sales", "gym");

        // Clave primaria
        builder.HasKey(s => s.SaleId);

        // Propiedades
        builder.Property(s => s.SaleId)
            .HasColumnName("SaleId")
            .IsRequired();

        builder.Property(s => s.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(s => s.MemberId)
            .HasColumnName("MemberId");

        builder.Property(s => s.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(s => s.PaymentMethodId)
            .HasColumnName("PaymentMethodId")
            .IsRequired();

        builder.Property(s => s.TicketNumber)
            .HasColumnName("TicketNumber")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.SaleDate)
            .HasColumnName("SaleDate")
            .IsRequired();

        builder.Property(s => s.Subtotal)
            .HasColumnName("Subtotal")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(s => s.DiscountAmount)
            .HasColumnName("DiscountAmount")
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(s => s.TaxAmount)
            .HasColumnName("TaxAmount")
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(s => s.Total)
            .HasColumnName("Total")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.PaymentReference)
            .HasColumnName("PaymentReference")
            .HasMaxLength(100);

        builder.Property(s => s.Notes)
            .HasColumnName("Notes");

        // Campos de auditoría
        builder.Property(s => s.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(s => s.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(s => s.TicketNumber)
            .IsUnique()
            .HasDatabaseName("IX_Sales_TicketNumber");

        builder.HasIndex(s => s.BranchId)
            .HasDatabaseName("IX_Sales_BranchId");

        builder.HasIndex(s => s.SaleDate)
            .HasDatabaseName("IX_Sales_SaleDate");

        // Relaciones
        builder.HasOne(s => s.Branch)
            .WithMany(b => b.Sales)
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Member)
            .WithMany(m => m.Sales)
            .HasForeignKey(s => s.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sales)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.PaymentMethod)
            .WithMany(pm => pm.Sales)
            .HasForeignKey(s => s.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}