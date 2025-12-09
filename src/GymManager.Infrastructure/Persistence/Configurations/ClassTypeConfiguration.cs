using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad ClassType
/// </summary>
public class ClassTypeConfiguration : IEntityTypeConfiguration<ClassType>
{
    public void Configure(EntityTypeBuilder<ClassType> builder)
    {
        // Tabla y Schema
        builder.ToTable("ClassTypes", "gym");

        // Clave primaria
        builder.HasKey(ct => ct.ClassTypeId);

        // Propiedades
        builder.Property(ct => ct.ClassTypeId)
            .HasColumnName("ClassTypeId")
            .IsRequired();

        builder.Property(ct => ct.Name)
            .HasColumnName("Name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ct => ct.Description)
            .HasColumnName("Description")
            .HasMaxLength(500);

        builder.Property(ct => ct.DurationMinutes)
            .HasColumnName("DurationMinutes")
            .HasDefaultValue(60);

        builder.Property(ct => ct.MaxCapacity)
            .HasColumnName("MaxCapacity")
            .HasDefaultValue(20);

        builder.Property(ct => ct.Color)
            .HasColumnName("Color")
            .HasMaxLength(7)
            .HasDefaultValue("#3B82F6");

        builder.Property(ct => ct.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasDefaultValue(true);

        // Campos de auditoría
        builder.Property(ct => ct.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(ct => ct.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(ct => ct.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(ct => ct.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(ct => ct.Name)
            .IsUnique()
            .HasDatabaseName("IX_ClassTypes_Name");
    }
}