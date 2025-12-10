using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// CLASS SCHEDULES (19_class_schedules.sql)
// ============================================================
public class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
{
    public void Configure(EntityTypeBuilder<ClassSchedule> builder)
    {
        builder.ToTable("classschedules");

        builder.HasKey(cs => cs.ScheduleId);

        builder.Property(cs => cs.DayOfWeek)
            .IsRequired();

        builder.Property(cs => cs.StartTime)
            .IsRequired();

        builder.Property(cs => cs.EndTime)
            .IsRequired();

        // C#: MaxCapacity -> SQL: capacity
        builder.Property(cs => cs.MaxCapacity)
            .HasColumnName("capacity");

        // C#: Room -> SQL: room_location
        builder.Property(cs => cs.Room)
            .HasColumnName("room_location")
            .HasMaxLength(50);

        // C#: IsAvailable -> SQL: is_active
        builder.Property(cs => cs.IsAvailable)
            .HasColumnName("is_active");

        // C#: InstructorId -> SQL: trainer_id
        builder.Property(cs => cs.InstructorId)
            .HasColumnName("trainer_id");

        // Indices
        builder.HasIndex(cs => cs.BranchId);
        builder.HasIndex(cs => cs.ClassTypeId);
        builder.HasIndex(cs => cs.InstructorId);
        builder.HasIndex(cs => new { cs.DayOfWeek, cs.StartTime });

        // Relaciones
        builder.HasOne(cs => cs.ClassType)
            .WithMany(ct => ct.ClassSchedules)
            .HasForeignKey(cs => cs.ClassTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Branch)
            .WithMany(b => b.ClassSchedules)
            .HasForeignKey(cs => cs.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cs => cs.Instructor)
            .WithMany()
            .HasForeignKey(cs => cs.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria y calculadas
        builder.Ignore(cs => cs.DeletedBy);
        builder.Ignore(cs => cs.IsDeleted);
        builder.Ignore(cs => cs.DayName); // Propiedad calculada

        // Filtro soft delete
        builder.HasQueryFilter(cs => cs.DeletedAt == null);
    }
}