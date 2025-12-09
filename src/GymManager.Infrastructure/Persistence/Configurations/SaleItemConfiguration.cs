using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad SaleItem
/// </summary>
public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        // Tabla y Schema
        builder.ToTable("SaleItems", "gym");

        // Clave primaria
        builder.HasKey(si => si.SaleItemId);

        // Propiedades
        builder.Property(si => si.SaleItemId)
            .HasColumnName("SaleItemId")
            .IsRequired();

        builder.Property(si => si.SaleId)
            .HasColumnName("SaleId")
            .IsRequired();

        builder.Property(si => si.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(si => si.Quantity)
            .HasColumnName("Quantity")
            .HasDefaultValue(1);

        builder.Property(si => si.UnitPrice)
            .HasColumnName("UnitPrice")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasColumnName("Discount")
            .HasPrecision(10, 2)
            .HasDefaultValue(0);

        builder.Property(si => si.Subtotal)
            .HasColumnName("Subtotal")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(si => si.IsRental)
            .HasColumnName("IsRental")
            .HasDefaultValue(false);

        builder.Property(si => si.RentalReturnDate)
            .HasColumnName("RentalReturnDate");

        builder.Property(si => si.ActualReturnDate)
            .HasColumnName("ActualReturnDate");

        builder.Property(si => si.DepositAmount)
            .HasColumnName("DepositAmount")
            .HasPrecision(10, 2);

        builder.Property(si => si.DepositReturned)
            .HasColumnName("DepositReturned")
            .HasDefaultValue(false);

        // Ignorar propiedades calculadas
        builder.Ignore(si => si.IsPendingReturn);

        // Campos de auditoría
        builder.Property(si => si.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(si => si.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(si => si.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(si => si.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(si => si.SaleId)
            .HasDatabaseName("IX_SaleItems_SaleId");

        builder.HasIndex(si => si.ProductId)
            .HasDatabaseName("IX_SaleItems_ProductId");

        // Relaciones
        builder.HasOne(si => si.Sale)
            .WithMany(s => s.SaleItems)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.Product)
            .WithMany(p => p.SaleItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}