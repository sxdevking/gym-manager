using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManager.Infrastructure.Persistence.Configurations;

// ============================================================
// MEMBERSHIPS (09_memberships.sql)
// ============================================================
public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("memberships");

        builder.HasKey(m => m.MembershipId);

        builder.Property(m => m.StartDate)
            .IsRequired();

        builder.Property(m => m.EndDate)
            .IsRequired();

        // Enum Status
        builder.Property(m => m.Status)
            .HasConversion(
                v => v.ToString().ToUpper(),
                v => Enum.Parse<MembershipStatus>(v, true))
            .HasMaxLength(20);

        builder.Property(m => m.PricePaid)
            .HasPrecision(10, 2);

        // C#: FreezeDaysUsed -> SQL: frozen_days_used
        builder.Property(m => m.FreezeDaysUsed)
            .HasColumnName("frozen_days_used");

        // C#: FreezeStartDate -> SQL: frozen_at
        builder.Property(m => m.FreezeStartDate)
            .HasColumnName("frozen_at");

        builder.Property(m => m.Notes)
            .HasColumnType("text");

        // Indices
        builder.HasIndex(m => m.MemberId);
        builder.HasIndex(m => m.PlanId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => new { m.StartDate, m.EndDate });

        // Relaciones
        builder.HasOne(m => m.Member)
            .WithMany(mb => mb.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Plan)
            .WithMany(mp => mp.Memberships)
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignorar propiedades de auditoria que no existen
        builder.Ignore(m => m.DeletedBy);
        builder.Ignore(m => m.IsDeleted);

        // Ignorar propiedades calculadas
        builder.Ignore(m => m.IsCurrentlyActive);
        builder.Ignore(m => m.DaysRemaining);

        // Filtro soft delete
        builder.HasQueryFilter(m => m.DeletedAt == null);
    }
}