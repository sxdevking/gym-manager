using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// ROLES (04_roles.sql)
// ============================================================
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.RoleId);

        // C#: Name -> SQL: role_name
        builder.Property(r => r.Name)
            .HasColumnName("role_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(255);

        // Indices
        builder.HasIndex(r => r.Name).IsUnique();

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(r => r.DeletedBy);
        builder.Ignore(r => r.IsDeleted);
        builder.Ignore(r => r.DeletedAt);
        builder.Ignore(r => r.UpdatedAt);
        builder.Ignore(r => r.UpdatedBy);
        builder.Ignore(r => r.CreatedBy);
    }
}