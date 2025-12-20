using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de la entidad Member para EF Core
/// Mapea todas las propiedades de la tabla gym.members
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.MemberId);

        // ═══════════════════════════════════════════════════════════════
        // INFORMACION BASICA
        // ═══════════════════════════════════════════════════════════════

        builder.Property(m => m.MemberCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Email)
            .HasMaxLength(100);

        builder.Property(m => m.Phone)
            .HasMaxLength(20);

        builder.Property(m => m.MobilePhone)
            .HasMaxLength(20);

        // ═══════════════════════════════════════════════════════════════
        // DATOS PERSONALES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(m => m.BirthDate);

        // Gender como enum nativo de PostgreSQL
        builder.Property(m => m.Gender)
            .HasColumnName("gender");

        builder.Property(m => m.IdDocumentType)
            .HasMaxLength(20);

        builder.Property(m => m.IdDocumentNumber)
            .HasMaxLength(50);

        builder.Property(m => m.PhotoPath)
            .HasMaxLength(500);

        // ═══════════════════════════════════════════════════════════════
        // DIRECCION
        // ═══════════════════════════════════════════════════════════════

        builder.Property(m => m.Address)
            .HasMaxLength(255);

        builder.Property(m => m.City)
            .HasMaxLength(100);

        builder.Property(m => m.State)
            .HasMaxLength(100);

        builder.Property(m => m.PostalCode)
            .HasMaxLength(20);

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO DE EMERGENCIA
        // ═══════════════════════════════════════════════════════════════

        builder.Property(m => m.EmergencyContactName)
            .HasMaxLength(100);

        builder.Property(m => m.EmergencyContactPhone)
            .HasMaxLength(20);

        builder.Property(m => m.EmergencyContactRelationship)
            .HasMaxLength(50);

        // ═══════════════════════════════════════════════════════════════
        // INFORMACION ADICIONAL
        // ═══════════════════════════════════════════════════════════════

        builder.Property(m => m.MedicalNotes)
            .HasColumnType("text");

        builder.Property(m => m.Notes)
            .HasColumnType("text");

        builder.Property(m => m.RegistrationDate)
            .HasDefaultValueSql("CURRENT_DATE");

        // ═══════════════════════════════════════════════════════════════
        // INDICES
        // ═══════════════════════════════════════════════════════════════

        builder.HasIndex(m => m.MemberCode).IsUnique();
        builder.HasIndex(m => m.Email);
        builder.HasIndex(m => m.Phone);
        builder.HasIndex(m => new { m.LastName, m.FirstName });
        builder.HasIndex(m => m.BranchId);
        builder.HasIndex(m => m.ReferredByMemberId);

        // ═══════════════════════════════════════════════════════════════
        // RELACIONES
        // ═══════════════════════════════════════════════════════════════

        // Relacion con Branch
        builder.HasOne(m => m.Branch)
            .WithMany(b => b.Members)
            .HasForeignKey(m => m.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacion con miembro que lo refirio (self-reference)
        builder.HasOne(m => m.ReferredByMember)
            .WithMany(m => m.ReferredMembers)
            .HasForeignKey(m => m.ReferredByMemberId)
            .OnDelete(DeleteBehavior.SetNull);

        // ═══════════════════════════════════════════════════════════════
        // PROPIEDADES IGNORADAS
        // ═══════════════════════════════════════════════════════════════

        // Propiedades calculadas
        builder.Ignore(m => m.FullName);
        builder.Ignore(m => m.Age);

        // Propiedades obsoletas (para compatibilidad)
#pragma warning disable CS0618
        builder.Ignore(m => m.EmergencyContact);
        builder.Ignore(m => m.EmergencyPhone);
        builder.Ignore(m => m.PhotoBase64);
        builder.Ignore(m => m.Barcode);
#pragma warning restore CS0618

        // Propiedades de AuditableEntity que no existen en esta tabla
        builder.Ignore(m => m.DeletedBy);
        builder.Ignore(m => m.IsDeleted);

        // ═══════════════════════════════════════════════════════════════
        // FILTRO SOFT DELETE
        // ═══════════════════════════════════════════════════════════════

        builder.HasQueryFilter(m => m.DeletedAt == null);
    }
}