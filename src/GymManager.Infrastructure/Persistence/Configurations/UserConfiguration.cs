using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad User
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Tabla y Schema
        builder.ToTable("Users", "gym");

        // Clave primaria
        builder.HasKey(u => u.UserId);

        // Propiedades
        builder.Property(u => u.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(u => u.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(u => u.Username)
            .HasColumnName("Username")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.FullName)
            .HasColumnName("FullName")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Phone)
            .HasColumnName("Phone")
            .HasMaxLength(20);

        builder.Property(u => u.AvatarBase64)
            .HasColumnName("AvatarBase64");

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("LastLoginAt");

        builder.Property(u => u.FailedLoginAttempts)
            .HasColumnName("FailedLoginAttempts")
            .HasDefaultValue(0);

        builder.Property(u => u.LockoutUntil)
            .HasColumnName("LockoutUntil");

        // Campos de auditoría
        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(u => u.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("IX_Users_Username");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.BranchId)
            .HasDatabaseName("IX_Users_BranchId");

        // Relaciones
        builder.HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}