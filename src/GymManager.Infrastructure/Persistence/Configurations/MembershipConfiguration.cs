using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Membership
/// </summary>
public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        // Tabla y Schema
        builder.ToTable("Memberships", "gym");

        // Clave primaria
        builder.HasKey(m => m.MembershipId);

        // Propiedades
        builder.Property(m => m.MembershipId)
            .HasColumnName("MembershipId")
            .IsRequired();

        builder.Property(m => m.MemberId)
            .HasColumnName("MemberId")
            .IsRequired();

        builder.Property(m => m.PlanId)
            .HasColumnName("PlanId")
            .IsRequired();

        builder.Property(m => m.StartDate)
            .HasColumnName("StartDate")
            .IsRequired();

        builder.Property(m => m.EndDate)
            .HasColumnName("EndDate")
            .IsRequired();

        builder.Property(m => m.Status)
            .HasColumnName("Status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.PricePaid)
            .HasColumnName("PricePaid")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(m => m.FreezeDaysUsed)
            .HasColumnName("FreezeDaysUsed")
            .HasDefaultValue(0);

        builder.Property(m => m.FreezeStartDate)
            .HasColumnName("FreezeStartDate");

        builder.Property(m => m.Notes)
            .HasColumnName("Notes");

        // Ignorar propiedades calculadas
        builder.Ignore(m => m.IsCurrentlyActive);
        builder.Ignore(m => m.DaysRemaining);

        // Campos de auditoría
        builder.Property(m => m.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(m => m.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("UpdatedAt");

        builder.Property(m => m.DeletedAt)
            .HasColumnName("DeletedAt");

        // Índices
        builder.HasIndex(m => m.MemberId)
            .HasDatabaseName("IX_Memberships_MemberId");

        builder.HasIndex(m => m.PlanId)
            .HasDatabaseName("IX_Memberships_PlanId");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("IX_Memberships_Status");

        // Relaciones
        builder.HasOne(m => m.Member)
            .WithMany(mb => mb.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Plan)
            .WithMany(mp => mp.Memberships)
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}