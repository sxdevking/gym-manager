using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// USERS (05_users.sql)
// ============================================================
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.UserId);

        // C#: Username -> SQL: employee_code (mapeo aproximado)
        builder.Property(u => u.Username)
            .HasColumnName("employee_code")
            .HasMaxLength(20);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        // C#: FullName -> SQL tiene first_name y last_name separados
        // Mapeamos a first_name (SQL concatenara o ajustaremos despues)
        builder.Property(u => u.FullName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        // C#: AvatarBase64 -> SQL: avatar_path
        builder.Property(u => u.AvatarBase64)
            .HasColumnName("avatar_path")
            .HasMaxLength(500);

        builder.Property(u => u.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(u => u.FailedLoginAttempts)
            .HasColumnName("failed_login_attempts");

        // C#: LockoutUntil -> SQL: locked_until
        builder.Property(u => u.LockoutUntil)
            .HasColumnName("locked_until");

        // Indices
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();

        // Relaciones
        builder.HasOne(u => u.Branch)
            .WithMany(b => b.Users)
            .HasForeignKey(u => u.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(u => u.DeletedBy);
        builder.Ignore(u => u.IsDeleted);

        // Filtro soft delete
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}