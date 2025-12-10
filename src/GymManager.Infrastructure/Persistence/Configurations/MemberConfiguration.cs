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

        // Propiedades basicas
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

        builder.Property(m => m.Address)
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

        // C#: Barcode -> NO existe en SQL, ignorar
        builder.Ignore(m => m.Barcode);

        builder.Property(m => m.Notes)
            .HasColumnType("text");

        // Enum Gender -> string de 1 caracter
        builder.Property(m => m.Gender)
            .HasConversion(
                v => v.HasValue ? v.Value.ToString().Substring(0, 1).ToUpper() : null,
                v => ParseGender(v))
            .HasMaxLength(1);

        // Indices
        builder.HasIndex(m => m.MemberCode).IsUnique();
        builder.HasIndex(m => m.Email);
        builder.HasIndex(m => m.Phone);
        builder.HasIndex(m => new { m.LastName, m.FirstName });
        builder.HasIndex(m => m.BranchId);

        // Relaciones
        builder.HasOne(m => m.Branch)
            .WithMany(b => b.Members)
            .HasForeignKey(m => m.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria que no existen en SQL
        builder.Ignore(m => m.DeletedBy);
        builder.Ignore(m => m.IsDeleted);

        // Ignorar propiedad calculada
        builder.Ignore(m => m.FullName);

        // Filtro soft delete
        builder.HasQueryFilter(m => m.DeletedAt == null);
    }

    private static Gender? ParseGender(string? value)
    {
        return value?.ToUpper() switch
        {
            "M" => Gender.Male,
            "F" => Gender.Female,
            "O" => Gender.Other,
            _ => null
        };
    }
}