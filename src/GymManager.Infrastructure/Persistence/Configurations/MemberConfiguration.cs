using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Member
/// </summary>
public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        // Tabla y Schema
        builder.ToTable("Members", "gym");

        // Clave primaria
        builder.HasKey(m => m.MemberId);

        // Propiedades
        builder.Property(m => m.MemberId)
            .HasColumnName("MemberId")
            .IsRequired();

        builder.Property(m => m.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(m => m.MemberCode)
            .HasColumnName("MemberCode")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Email)
            .HasColumnName("Email")
            .HasMaxLength(100);

        builder.Property(m => m.Phone)
            .HasColumnName("Phone")
            .HasMaxLength(20);

        builder.Property(m => m.BirthDate)
            .HasColumnName("BirthDate");

        builder.Property(m => m.Gender)
            .HasColumnName("Gender")
            .HasConversion<string>()
            .HasMaxLength(1);

        builder.Property(m => m.Address)
            .HasColumnName("Address")
            .HasMaxLength(255);

        builder.Property(m => m.EmergencyContact)
            .HasColumnName("EmergencyContact")
            .HasMaxLength(100);

        builder.Property(m => m.EmergencyPhone)
            .HasColumnName("EmergencyPhone")
            .HasMaxLength(20);

        builder.Property(m => m.PhotoBase64)
            .HasColumnName("PhotoBase64");

        builder.Property(m => m.Barcode)
            .HasColumnName("Barcode")
            .HasMaxLength(50);

        builder.Property(m => m.Notes)
            .HasColumnName("Notes");

        // Ignorar propiedades calculadas
        builder.Ignore(m => m.FullName);

        // Campos de auditoría
        builder.Property(m => m.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(m => m.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(m => m.MemberCode)
            .IsUnique()
            .HasDatabaseName("IX_Members_MemberCode");

        builder.HasIndex(m => m.Email)
            .HasDatabaseName("IX_Members_Email");

        builder.HasIndex(m => m.BranchId)
            .HasDatabaseName("IX_Members_BranchId");

        builder.HasIndex(m => m.Barcode)
            .HasDatabaseName("IX_Members_Barcode");

        // Relaciones
        builder.HasOne(m => m.Branch)
            .WithMany(b => b.Members)
            .HasForeignKey(m => m.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}