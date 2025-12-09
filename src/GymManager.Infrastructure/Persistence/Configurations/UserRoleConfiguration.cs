using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad UserRole (tabla intermedia)
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Tabla y Schema
        builder.ToTable("UserRoles", "gym");

        // Clave primaria
        builder.HasKey(ur => ur.UserRoleId);

        // Propiedades
        builder.Property(ur => ur.UserRoleId)
            .HasColumnName("UserRoleId")
            .IsRequired();

        builder.Property(ur => ur.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .HasColumnName("RoleId")
            .IsRequired();

        builder.Property(ur => ur.AssignedAt)
            .HasColumnName("AssignedAt")
            .IsRequired();

        // Campos de auditoría
        builder.Property(ur => ur.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(ur => ur.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(ur => ur.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(ur => ur.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("IX_UserRoles_UserId_RoleId");

        // Relaciones
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}