using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad ClassEnrollment
/// </summary>
public class ClassEnrollmentConfiguration : IEntityTypeConfiguration<ClassEnrollment>
{
    public void Configure(EntityTypeBuilder<ClassEnrollment> builder)
    {
        // Tabla y Schema
        builder.ToTable("ClassEnrollments", "gym");

        // Clave primaria
        builder.HasKey(ce => ce.EnrollmentId);

        // Propiedades
        builder.Property(ce => ce.EnrollmentId)
            .HasColumnName("EnrollmentId")
            .IsRequired();

        builder.Property(ce => ce.ScheduleId)
            .HasColumnName("ScheduleId")
            .IsRequired();

        builder.Property(ce => ce.MemberId)
            .HasColumnName("MemberId")
            .IsRequired();

        builder.Property(ce => ce.ClassDate)
            .HasColumnName("ClassDate")
            .IsRequired();

        builder.Property(ce => ce.EnrolledAt)
            .HasColumnName("EnrolledAt")
            .IsRequired();

        builder.Property(ce => ce.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(ce => ce.CheckedInAt)
            .HasColumnName("CheckedInAt");

        builder.Property(ce => ce.Notes)
            .HasColumnName("Notes");

        // Campos de auditoría
        builder.Property(ce => ce.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(ce => ce.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(ce => ce.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(ce => ce.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(ce => ce.ScheduleId)
            .HasDatabaseName("IX_ClassEnrollments_ScheduleId");

        builder.HasIndex(ce => ce.MemberId)
            .HasDatabaseName("IX_ClassEnrollments_MemberId");

        builder.HasIndex(ce => new { ce.ScheduleId, ce.MemberId, ce.ClassDate })
            .IsUnique()
            .HasDatabaseName("IX_ClassEnrollments_Unique");

        // Relaciones
        builder.HasOne(ce => ce.ClassSchedule)
            .WithMany(cs => cs.ClassEnrollments)
            .HasForeignKey(ce => ce.ScheduleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ce => ce.Member)
            .WithMany(m => m.ClassEnrollments)
            .HasForeignKey(ce => ce.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}