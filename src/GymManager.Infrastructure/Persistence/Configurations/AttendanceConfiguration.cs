using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Attendance
/// </summary>
public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        // Tabla y Schema
        builder.ToTable("Attendances", "gym");

        // Clave primaria
        builder.HasKey(a => a.AttendanceId);

        // Propiedades
        builder.Property(a => a.AttendanceId)
            .HasColumnName("AttendanceId")
            .IsRequired();

        builder.Property(a => a.MemberId)
            .HasColumnName("MemberId")
            .IsRequired();

        builder.Property(a => a.BranchId)
            .HasColumnName("BranchId")
            .IsRequired();

        builder.Property(a => a.CheckInTime)
            .HasColumnName("CheckInTime")
            .IsRequired();

        builder.Property(a => a.CheckOutTime)
            .HasColumnName("CheckOutTime");

        builder.Property(a => a.CheckInMethod)
            .HasColumnName("CheckInMethod")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.DurationMinutes)
            .HasColumnName("DurationMinutes");

        builder.Property(a => a.Notes)
            .HasColumnName("Notes");

        // Ignorar propiedades calculadas
        builder.Ignore(a => a.IsCurrentlyInGym);

        // Campos de auditoría
        builder.Property(a => a.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(a => a.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(a => a.MemberId)
            .HasDatabaseName("IX_Attendances_MemberId");

        builder.HasIndex(a => a.BranchId)
            .HasDatabaseName("IX_Attendances_BranchId");

        builder.HasIndex(a => a.CheckInTime)
            .HasDatabaseName("IX_Attendances_CheckInTime");

        // Relaciones
        builder.HasOne(a => a.Member)
            .WithMany(m => m.Attendances)
            .HasForeignKey(a => a.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Branch)
            .WithMany()
            .HasForeignKey(a => a.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}