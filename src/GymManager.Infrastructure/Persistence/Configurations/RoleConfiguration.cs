using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Role
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Tabla y Schema
        builder.ToTable("Roles", "gym");

        // Clave primaria
        builder.HasKey(r => r.RoleId);

        // Propiedades
        builder.Property(r => r.RoleId)
            .HasColumnName("RoleId")
            .IsRequired();

        builder.Property(r => r.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasMaxLength(255);

        // Campos de auditoría
        builder.Property(r => r.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(r => r.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");
    }
}