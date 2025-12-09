using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad PaymentMethod
/// </summary>
public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        // Tabla y Schema
        builder.ToTable("PaymentMethods", "gym");

        // Clave primaria
        builder.HasKey(pm => pm.PaymentMethodId);

        // Propiedades
        builder.Property(pm => pm.PaymentMethodId)
            .HasColumnName("PaymentMethodId")
            .IsRequired();

        builder.Property(pm => pm.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(pm => pm.Description)
            .HasColumnName("Description")
            .HasMaxLength(255);

        builder.Property(pm => pm.RequiresReference)
            .HasColumnName("RequiresReference")
            .HasDefaultValue(false);

        // Campos de auditoría
        builder.Property(pm => pm.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(pm => pm.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(pm => pm.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(pm => pm.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(pm => pm.Name)
            .IsUnique()
            .HasDatabaseName("IX_PaymentMethods_Name");
    }
}