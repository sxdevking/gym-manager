using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;
/// <summary>
/// Configuracion EF Core para la entidad Branch
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");

        builder.HasKey(b => b.BranchId);

        // ═══════════════════════════════════════════════════════════
        // MAPEO DE PROPIEDADES A COLUMNAS
        // ═══════════════════════════════════════════════════════════

        builder.Property(b => b.BranchId)
            .HasColumnName("branch_id");

        builder.Property(b => b.LicenseId)
            .HasColumnName("license_id")
            .IsRequired();

        builder.Property(b => b.BranchCode)
            .HasColumnName("branch_code")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(b => b.BranchName)
            .HasColumnName("branch_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Address)
            .HasColumnName("address")
            .HasMaxLength(255);

        builder.Property(b => b.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(b => b.State)
            .HasColumnName("state")
            .HasMaxLength(100);

        builder.Property(b => b.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20);

        builder.Property(b => b.Country)
            .HasColumnName("country")
            .HasMaxLength(50);

        builder.Property(b => b.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(b => b.IsHeadquarters)
            .HasColumnName("is_headquarters");

        // ═══════════════════════════════════════════════════════════
        // CAMPOS DE AUDITORIA (todos son UUID en PostgreSQL)
        // ═══════════════════════════════════════════════════════════

        builder.Property(b => b.IsActive)
            .HasColumnName("is_active");

        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at");

        // CreatedBy es UUID en PostgreSQL
        builder.Property(b => b.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at");

        // UpdatedBy es UUID en PostgreSQL
        builder.Property(b => b.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("deleted_at");

        // DeletedBy NO existe en la tabla branches de PostgreSQL - IGNORAR
        builder.Ignore(b => b.DeletedBy);

        // IsDeleted es propiedad calculada - IGNORAR
        builder.Ignore(b => b.IsDeleted);

        // ═══════════════════════════════════════════════════════════
        // INDICES
        // ═══════════════════════════════════════════════════════════

        builder.HasIndex(b => b.BranchCode)
            .IsUnique()
            .HasDatabaseName("uq_branches_branch_code");

        builder.HasIndex(b => b.LicenseId)
            .HasDatabaseName("idx_branches_license_id");

        builder.HasIndex(b => b.IsActive)
            .HasDatabaseName("idx_branches_is_active");

        builder.HasIndex(b => b.City)
            .HasDatabaseName("idx_branches_city");

        // ═══════════════════════════════════════════════════════════
        // RELACIONES
        // ═══════════════════════════════════════════════════════════

        builder.HasOne(b => b.License)
            .WithMany(l => l.Branches)
            .HasForeignKey(b => b.LicenseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Filtro soft delete
        builder.HasQueryFilter(b => b.DeletedAt == null);
    }
}
