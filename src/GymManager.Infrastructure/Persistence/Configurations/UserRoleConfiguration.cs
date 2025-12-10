using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// USER ROLES (06_user_roles.sql)
// ============================================================
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("userroles");

        builder.HasKey(ur => ur.UserRoleId);

        builder.Property(ur => ur.AssignedAt)
            .HasColumnName("assigned_at");

        // Relaciones
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indice unico
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(ur => ur.DeletedBy);
        builder.Ignore(ur => ur.IsDeleted);
        builder.Ignore(ur => ur.DeletedAt);
        builder.Ignore(ur => ur.UpdatedAt);
        builder.Ignore(ur => ur.UpdatedBy);
        builder.Ignore(ur => ur.CreatedBy);
        builder.Ignore(ur => ur.CreatedAt);
    }
}
