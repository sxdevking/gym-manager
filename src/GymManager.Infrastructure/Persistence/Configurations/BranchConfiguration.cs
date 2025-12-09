using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Branch
/// </summary>
public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        // Tabla y Schema
        builder.ToTable("Branches", "gym");

        // Clave primaria
        builder.HasKey(b => b.BranchId);

        // Propiedades
        builder.Property(b => b.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(b => b.Code)
            .HasColumnName("Code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(b => b.Name)
            .HasColumnName("Name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.Address)
            .HasColumnName("Address")
            .HasMaxLength(255);

        builder.Property(b => b.Phone)
            .HasColumnName("Phone")
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasColumnName("Email")
            .HasMaxLength(100);

        builder.Property(b => b.OpenTime)
            .HasColumnName("OpenTime");

        builder.Property(b => b.CloseTime)
            .HasColumnName("CloseTime");

        // Campos de auditoría
        builder.Property(b => b.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(b => b.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(b => b.Code)
            .IsUnique()
            .HasDatabaseName("IX_Branches_Code");

        // Relación 1:1 con BranchSettings
        builder.HasOne(b => b.Settings)
            .WithOne(bs => bs.Branch)
            .HasForeignKey<BranchSettings>(bs => bs.BranchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}