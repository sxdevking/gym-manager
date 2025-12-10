using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// CLASS ENROLLMENTS (20_class_enrollments.sql)
// ============================================================
public class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
{
    public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
    {
        builder.ToTable("classenrollments");

        builder.HasKey(ce => ce.EnrollmentId);

        builder.Property(ce => ce.ClassDate)
            .IsRequired();

        builder.Property(ce => ce.EnrolledAt)
            .IsRequired();

        // Enum Status
        builder.Property(ce => ce.Status)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<EnrollmentStatus>(v, true))
            .HasMaxLength(20);

        // C#: CheckedInAt -> SQL: attended_at
        builder.Property(ce => ce.CheckedInAt)
            .HasColumnName("attended_at");

        builder.Property(ce => ce.Notes)
            .HasMaxLength(255);

        // Indice unico
        builder.HasIndex(ce => new { ce.ScheduleId, ce.MemberId, ce.ClassDate }).IsUnique();

        // Indices adicionales
        builder.HasIndex(ce => ce.ScheduleId);
        builder.HasIndex(ce => ce.MemberId);
        builder.HasIndex(ce => ce.ClassDate);
        builder.HasIndex(ce => ce.Status);

        // Relaciones
        builder.HasOne(ce => ce.ClassSchedule)
            .WithMany(cs => cs.ClassEnrollments)
            .HasForeignKey(ce => ce.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ce => ce.Member)
            .WithMany(m => m.ClassEnrollments)
            .HasForeignKey(ce => ce.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar propiedades de auditoria
        builder.Ignore(ce => ce.DeletedBy);
        builder.Ignore(ce => ce.IsDeleted);
        builder.Ignore(ce => ce.DeletedAt);
        builder.Ignore(ce => ce.UpdatedAt);
        builder.Ignore(ce => ce.UpdatedBy);
        builder.Ignore(ce => ce.CreatedBy);
        builder.Ignore(ce => ce.CreatedAt);

        // Filtro soft delete
        builder.HasQueryFilter(ce => ce.DeletedAt == null);
    }
}