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

        // C#: CheckInTime -> SQL: check_in_at
        builder.Property(a => a.CheckInTime)
            .HasColumnName("check_in_at")
            .IsRequired();

        // C#: CheckOutTime -> SQL: check_out_at
        builder.Property(a => a.CheckOutTime)
            .HasColumnName("check_out_at");

        // Enum CheckInMethod
        builder.Property(a => a.CheckInMethod)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<CheckInMethod>(v, true))
            .HasMaxLength(20);

        builder.Property(a => a.DurationMinutes);

        builder.Property(a => a.Notes)
            .HasMaxLength(255);

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
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria y calculadas
        builder.Ignore(a => a.DeletedBy);
        builder.Ignore(a => a.IsDeleted);
        builder.Ignore(a => a.IsCurrentlyInGym);

        // Filtro soft delete
        builder.HasQueryFilter(a => a.DeletedAt == null);
    }
}
