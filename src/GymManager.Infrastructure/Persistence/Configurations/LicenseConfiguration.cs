using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de la entidad License para EF Core
/// Basado en: 01_licenses.sql
/// </summary>
public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.ToTable("licenses");

        builder.HasKey(l => l.LicenseId);

        // ═══════════════════════════════════════════════════════════
        // MAPEO DE PROPIEDADES A COLUMNAS
        // ═══════════════════════════════════════════════════════════

        builder.Property(l => l.LicenseId)
            .HasColumnName("license_id");

        builder.Property(l => l.LicenseKey)
            .HasColumnName("license_key")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(l => l.HardwareId)
            .HasColumnName("hardware_id")
            .IsRequired()
            .HasMaxLength(128);

        // ENUM NATIVO DE POSTGRESQL
        builder.Property(l => l.LicenseType)
            .HasColumnName("license_type");

        builder.Property(l => l.MaxBranches)
            .HasColumnName("max_branches");

        builder.Property(l => l.MaxUsers)
            .HasColumnName("max_users");

        builder.Property(l => l.IssuedAt)
            .HasColumnName("issued_at");

        builder.Property(l => l.ExpiresAt)
            .HasColumnName("expires_at");

        builder.Property(l => l.LastValidation)
            .HasColumnName("last_validation");

        builder.Property(l => l.ValidationCount)
            .HasColumnName("validation_count");

        // ═══════════════════════════════════════════════════════════
        // AUDITORIA - licenses solo tiene created_at y updated_at
        // ═══════════════════════════════════════════════════════════

        builder.Property(l => l.IsActive)
            .HasColumnName("is_active");

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at");

        // IGNORAR - NO existen en la tabla licenses de PostgreSQL
        builder.Ignore(l => l.CreatedBy);
        builder.Ignore(l => l.UpdatedBy);
        builder.Ignore(l => l.DeletedBy);
        builder.Ignore(l => l.IsDeleted);
        builder.Ignore(l => l.DeletedAt);

        // ═══════════════════════════════════════════════════════════
        // INDICES
        // ═══════════════════════════════════════════════════════════

        builder.HasIndex(l => l.LicenseKey)
            .IsUnique()
            .HasDatabaseName("uq_licenses_license_key");

        builder.HasIndex(l => l.HardwareId)
            .IsUnique()
            .HasDatabaseName("uq_licenses_hardware_id");

        builder.HasIndex(l => l.IsActive)
            .HasDatabaseName("idx_licenses_is_active");

        builder.HasIndex(l => l.ExpiresAt)
            .HasDatabaseName("idx_licenses_expires_at");
    }
}

