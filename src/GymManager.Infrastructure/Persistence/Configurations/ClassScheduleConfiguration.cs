using GymManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad ClassSchedule
/// </summary>
public class ClassScheduleConfiguration : IEntityTypeConfiguration<ClassSchedule>
{
    public void Configure(EntityTypeBuilder<ClassSchedule> builder)
    {
        // Tabla y Schema
        builder.ToTable("ClassSchedules", "gym");

        // Clave primaria
        builder.HasKey(cs => cs.ScheduleId);

        // Propiedades
        builder.Property(cs => cs.ScheduleId)
            .HasColumnName("ScheduleId")
            .IsRequired();

        builder.Property(cs => cs.ClassTypeId)
            .HasColumnName("ClassTypeId")
            .IsRequired();

        builder.Property(cs => cs.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(cs => cs.InstructorId)
            .HasColumnName("InstructorId");

        builder.Property(cs => cs.DayOfWeek)
            .HasColumnName("DayOfWeek")
            .IsRequired();

        builder.Property(cs => cs.StartTime)
            .HasColumnName("StartTime")
            .IsRequired();

        builder.Property(cs => cs.EndTime)
            .HasColumnName("EndTime")
            .IsRequired();

        builder.Property(cs => cs.MaxCapacity)
            .HasColumnName("MaxCapacity")
            .IsRequired();

        builder.Property(cs => cs.Room)
            .HasColumnName("Room")
            .HasMaxLength(50);

        builder.Property(cs => cs.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasDefaultValue(true);

        // Ignorar propiedades calculadas
        builder.Ignore(cs => cs.DayName);

        // Campos de auditoría
        builder.Property(cs => cs.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(cs => cs.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(cs => cs.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(cs => cs.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(cs => cs.ClassTypeId)
            .HasDatabaseName("IX_ClassSchedules_ClassTypeId");

        builder.HasIndex(cs => cs.BranchId)
            .HasDatabaseName("IX_ClassSchedules_BranchId");

        builder.HasIndex(cs => cs.InstructorId)
            .HasDatabaseName("IX_ClassSchedules_InstructorId");

        // Relaciones
        builder.HasOne(cs => cs.ClassType)
            .WithMany(ct => ct.ClassSchedules)
            .HasForeignKey(cs => cs.ClassTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Branch)
            .WithMany(b => b.ClassSchedules)
            .HasForeignKey(cs => cs.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Instructor)
            .WithMany()
            .HasForeignKey(cs => cs.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}