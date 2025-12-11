using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion EF Core para la entidad Attendance
/// </summary>
public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("attendances");

        builder.HasKey(a => a.AttendanceId);

        builder.Property(a => a.AttendanceId)
            .HasColumnName("attendance_id");

        builder.Property(a => a.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        builder.Property(a => a.BranchId)
            .HasColumnName("branch_id")
            .IsRequired();

        // C#: CheckInTime -> SQL: check_in_at
        builder.Property(a => a.CheckInTime)
            .HasColumnName("check_in_at")
            .IsRequired();

        // C#: CheckOutTime -> SQL: check_out_at
        builder.Property(a => a.CheckOutTime)
            .HasColumnName("check_out_at");

        // ENUM NATIVO DE POSTGRESQL - NO usar HasConversion
        builder.Property(a => a.CheckInMethod)
            .HasColumnName("check_in_method");

        builder.Property(a => a.DurationMinutes)
            .HasColumnName("duration_minutes");

        builder.Property(a => a.Notes)
            .HasColumnName("notes")
            .HasMaxLength(255);

        // Auditoria
        builder.Property(a => a.IsActive)
            .HasColumnName("is_active");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        // Ignorar
        builder.Ignore(a => a.DeletedBy);
        builder.Ignore(a => a.IsDeleted);
        builder.Ignore(a => a.DeletedAt);
        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
        builder.Ignore(a => a.IsCurrentlyInGym);

        // Indices
        builder.HasIndex(a => a.MemberId);
        builder.HasIndex(a => a.BranchId);
        builder.HasIndex(a => a.CheckInTime);
        builder.HasIndex(a => new { a.BranchId, a.CheckInTime });

        // Relaciones
        builder.HasOne(a => a.Member)
            .WithMany(m => m.Attendances)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Branch)
            .WithMany(b => b.Attendances)
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

