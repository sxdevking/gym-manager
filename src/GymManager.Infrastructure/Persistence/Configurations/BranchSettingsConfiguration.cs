using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad BranchSettings
/// </summary>
public class BranchSettingsConfiguration : IEntityTypeConfiguration<BranchSettings>
{
    public void Configure(EntityTypeBuilder<BranchSettings> builder)
    {
        // Tabla y Schema
        builder.ToTable("BranchSettings", "gym");

        // Clave primaria (misma que BranchId - relación 1:1)
        builder.HasKey(bs => bs.BranchId);

        // Propiedades
        builder.Property(bs => bs.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(bs => bs.BusinessName)
            .HasColumnName("BusinessName")
            .HasMaxLength(100);

        builder.Property(bs => bs.LogoBase64)
            .HasColumnName("LogoBase64");

        builder.Property(bs => bs.PrimaryColor)
            .HasColumnName("PrimaryColor")
            .HasMaxLength(7)
            .HasDefaultValue("#3B82F6");

        builder.Property(bs => bs.SecondaryColor)
            .HasColumnName("SecondaryColor")
            .HasMaxLength(7)
            .HasDefaultValue("#1E293B");

        builder.Property(bs => bs.ReceiptFooter)
            .HasColumnName("ReceiptFooter")
            .HasMaxLength(500);

        builder.Property(bs => bs.TaxId)
            .HasColumnName("TaxId")
            .HasMaxLength(20);

        builder.Property(bs => bs.TicketInfo)
            .HasColumnName("TicketInfo")
            .HasMaxLength(500);

        // Campos de auditoría
        builder.Property(bs => bs.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(bs => bs.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(bs => bs.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(bs => bs.DeletedAt)
            .HasColumnName("DeletedAt");
    }
}