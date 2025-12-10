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

        // Propiedades
        builder.Property(l => l.LicenseKey)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(l => l.HardwareId)
            .IsRequired()
            .HasMaxLength(128);

        // Enum a string
        builder.Property(l => l.LicenseType)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<LicenseType>(v, true))
            .HasMaxLength(20);

        // Indices
        builder.HasIndex(l => l.LicenseKey).IsUnique();
        builder.HasIndex(l => l.HardwareId).IsUnique();

        // Ignorar propiedades de AuditableEntity que no existen en SQL
        builder.Ignore(l => l.DeletedBy);
        builder.Ignore(l => l.IsDeleted);

        // Filtro soft delete
        builder.HasQueryFilter(l => l.DeletedAt == null);
    }
}
