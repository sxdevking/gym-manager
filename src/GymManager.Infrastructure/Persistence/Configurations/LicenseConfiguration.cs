using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad License
/// </summary>
public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        // Tabla y Schema
        builder.ToTable("Licenses", "gym");

        // Clave primaria
        builder.HasKey(l => l.LicenseId);

        // Propiedades
        builder.Property(l => l.LicenseId)
            .HasColumnName("LicenseId")
            .IsRequired();

        builder.Property(l => l.LicenseKey)
            .HasColumnName("LicenseKey")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(l => l.HardwareId)
            .HasColumnName("HardwareId")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(l => l.LicenseType)
            .HasColumnName("LicenseType")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(l => l.MaxBranches)
            .HasColumnName("MaxBranches")
            .HasDefaultValue(1);

        builder.Property(l => l.MaxUsers)
            .HasColumnName("MaxUsers")
            .HasDefaultValue(3);

        builder.Property(l => l.IssuedAt)
            .HasColumnName("IssuedAt")
            .IsRequired();

        builder.Property(l => l.ExpiresAt)
            .HasColumnName("ExpiresAt");

        builder.Property(l => l.LastValidation)
            .HasColumnName("LastValidation");

        builder.Property(l => l.ValidationCount)
            .HasColumnName("ValidationCount")
            .HasDefaultValue(0);

        // Campos de auditoría
        builder.Property(l => l.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(l => l.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(l => l.HardwareId)
            .IsUnique()
            .HasDatabaseName("IX_Licenses_HardwareId");

        builder.HasIndex(l => l.LicenseKey)
            .IsUnique()
            .HasDatabaseName("IX_Licenses_LicenseKey");
    }
}