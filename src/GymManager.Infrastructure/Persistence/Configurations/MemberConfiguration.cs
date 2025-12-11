using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de la entidad Member para EF Core
/// IMPORTANTE: Mapeado segun la entidad C# real, NO segun el SQL completo
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.MemberId);

        // ═══════════════════════════════════════════════════════════
        // MAPEO DE PROPIEDADES A COLUMNAS
        // ═══════════════════════════════════════════════════════════

        builder.Property(m => m.MemberId)
            .HasColumnName("member_id");

        builder.Property(m => m.BranchId)
            .HasColumnName("branch_id")
            .IsRequired();

        builder.Property(m => m.MemberCode)
            .HasColumnName("member_code")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(m => m.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(m => m.BirthDate)
            .HasColumnName("birth_date");

        // ENUM NATIVO DE POSTGRESQL
        builder.Property(m => m.Gender)
            .HasColumnName("gender");

        builder.Property(m => m.Address)
            .HasColumnName("address")
            .HasMaxLength(255);

        // C#: EmergencyContact -> SQL: emergency_contact_name
        builder.Property(m => m.EmergencyContact)
            .HasColumnName("emergency_contact_name")
            .HasMaxLength(100);

        // C#: EmergencyPhone -> SQL: emergency_contact_phone
        builder.Property(m => m.EmergencyPhone)
            .HasColumnName("emergency_contact_phone")
            .HasMaxLength(20);

        // C#: PhotoBase64 -> SQL: photo_path
        builder.Property(m => m.PhotoBase64)
            .HasColumnName("photo_path")
            .HasMaxLength(500);

        builder.Property(m => m.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // ═══════════════════════════════════════════════════════════
        // AUDITORIA (created_by y updated_by son UUID)
        // ═══════════════════════════════════════════════════════════

        builder.Property(m => m.IsActive)
            .HasColumnName("is_active");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(m => m.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(m => m.DeletedAt)
            .HasColumnName("deleted_at");

        // IGNORAR - No existen en members
        builder.Ignore(m => m.Barcode);
        builder.Ignore(m => m.DeletedBy);
        builder.Ignore(m => m.IsDeleted);
        builder.Ignore(m => m.FullName);

        // ═══════════════════════════════════════════════════════════
        // INDICES
        // ═══════════════════════════════════════════════════════════

        builder.HasIndex(m => m.MemberCode)
            .IsUnique()
            .HasDatabaseName("uq_members_member_code");

        builder.HasIndex(m => m.Email)
            .HasDatabaseName("idx_members_email");

        builder.HasIndex(m => m.Phone)
            .HasDatabaseName("idx_members_phone");

        builder.HasIndex(m => new { m.LastName, m.FirstName })
            .HasDatabaseName("idx_members_name");

        builder.HasIndex(m => m.BranchId)
            .HasDatabaseName("idx_members_branch_id");

        builder.HasIndex(m => m.IsActive)
            .HasDatabaseName("idx_members_is_active");

        // ═══════════════════════════════════════════════════════════
        // RELACIONES
        // ═══════════════════════════════════════════════════════════

        builder.HasOne(m => m.Branch)
            .WithMany(b => b.Members)
            .HasForeignKey(m => m.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Filtro soft delete
        builder.HasQueryFilter(m => m.DeletedAt == null);
    }
}

