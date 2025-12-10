using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// CLASS TYPES (18_class_types.sql)
// ============================================================
public class ClassTypeConfiguration : IEntityTypeConfiguration<ClassType>
{
    public void Configure(EntityTypeBuilder<ClassType> builder)
    {
        builder.ToTable("classtypes");

        builder.HasKey(ct => ct.ClassTypeId);

        // C#: Name -> SQL: type_name
        builder.Property(ct => ct.Name)
            .HasColumnName("type_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ct => ct.Description)
            .HasColumnType("text");

        // C#: DurationMinutes -> SQL: default_duration_minutes
        builder.Property(ct => ct.DurationMinutes)
            .HasColumnName("default_duration_minutes");

        // C#: MaxCapacity -> SQL: default_capacity
        builder.Property(ct => ct.MaxCapacity)
            .HasColumnName("default_capacity");

        // C#: Color -> SQL: color_code
        builder.Property(ct => ct.Color)
            .HasColumnName("color_code")
            .HasMaxLength(7);

        // C#: IsAvailable -> SQL: is_active
        builder.Property(ct => ct.IsAvailable)
            .HasColumnName("is_active");

        // Indices
        builder.HasIndex(ct => ct.Name).IsUnique();

        // Ignorar propiedades de auditoria
        builder.Ignore(ct => ct.DeletedBy);
        builder.Ignore(ct => ct.IsDeleted);
        builder.Ignore(ct => ct.DeletedAt);
        builder.Ignore(ct => ct.UpdatedAt);
        builder.Ignore(ct => ct.UpdatedBy);
        builder.Ignore(ct => ct.CreatedBy);
    }
}